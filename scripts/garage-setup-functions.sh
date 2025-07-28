# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to print banner
print_banner() {
    echo -e "${GREEN}========================================${NC}"
    echo -e "${GREEN}    Garage S3 Container Setup Script   ${NC}"
    echo -e "${GREEN}========================================${NC}\n"
}

# Function to reset volume and containers
reset_volume() {
    log_info "Stopping and removing existing Garage container and volume..."
    docker compose down --volumes --remove-orphans 2>/dev/null || docker-compose down --volumes --remove-orphans 2>/dev/null || true
    docker volume rm "$GARAGE_VOLUME" 2>/dev/null || true
    log_success "Volume '$GARAGE_VOLUME' removed."
}

# Function to execute garage commands
garage_exec() {
    docker exec "$CONTAINER_NAME" /garage -c "$CONFIG_PATH" "$@"
}

# Function to execute garage commands directly (for when running on host)
garage_direct() {
    garage --config "$CONFIG_PATH" "$@"
}

# Function to check if container is running
check_container() {
    if ! docker ps --format "table {{.Names}}" | grep -q "^${CONTAINER_NAME}$"; then
        log_error "Container '$CONTAINER_NAME' is not running!"
        exit 1
    fi
    log_success "Container '$CONTAINER_NAME' is running"
}

# Function to get node ID
get_node_id() {
    log_info "Getting garage node ID..."

    local node_id=$(garage_exec status 2>/dev/null | awk '/^==== HEALTHY NODES ====/ { getline; getline; print $1 }')

    if [[ -z "$node_id" ]]; then
        log_error "Could not extract node ID from 'garage status'"
        garage_exec status
        exit 1
    fi

    log_success "Found node ID: $node_id"
    echo "$node_id"
}

# Function to setup garage layout
setup_layout() {
    local node_id="$1"

    if [[ -z "$node_id" ]]; then
        log_error "Node ID is empty, cannot setup layout"
        exit 1
    fi

    log_info "Checking if layout is already assigned..."

    # Check if layout is already configured - use simpler check
    local layout_output=$(garage_exec layout show 2>/dev/null || true)
    if [[ -n "$layout_output" ]] && echo "$layout_output" | grep -F "$node_id" >/dev/null 2>&1; then
        log_info "Layout already assigned to node $node_id"
        return 0
    fi

    log_info "Assigning layout to node $node_id..."

    # Use the proven working format from the original script
    garage_exec layout assign -z "$ZONE" -c "$CAPACITY" "$node_id"

    log_info "Applying layout version 1..."
    garage_exec layout apply --version 1

    log_success "Layout configured successfully"
}

# Function to create bucket
create_bucket() {
    log_info "Checking if bucket '$BUCKET_NAME' exists..."

    if garage_exec bucket list 2>/dev/null | grep -q "$BUCKET_NAME"; then
        log_info "Bucket '$BUCKET_NAME' already exists. Skipping creation."
        return 0
    fi

    log_info "Creating bucket '$BUCKET_NAME'..."
    garage_exec bucket create "$BUCKET_NAME"
    log_success "Bucket '$BUCKET_NAME' created"
}

# Function to create key
create_key() {
    log_info "Creating key '$KEY_NAME'..." >&2

    # Check if key already exists
    if garage_exec key list 2>/dev/null | grep -q "$KEY_NAME"; then
        log_info "Key '$KEY_NAME' already exists. Retrieving existing key info..." >&2
        local key_output=$(garage_exec key info "$KEY_NAME")
        
        # Extract Key ID from existing key info
        local access_key=$(echo "$key_output" | grep "Key ID:" | awk '{print $3}')
        
        if [[ -z "$access_key" ]]; then
            log_error "Failed to extract existing key ID" >&2
            echo "$key_output" >&2
            exit 1
        fi
        
        log_warning "Using existing key. Secret key cannot be retrieved for existing keys." >&2
        log_info "You may need to use the secret key from your previous setup or delete and recreate the key." >&2
        
        # For existing keys, we can't get the secret, so return empty secret
        echo "$access_key "
        return 0
    fi

    # Capture the key creation output - redirect stderr to avoid log pollution
    local key_output=$(garage_exec key create "$KEY_NAME" 2>/dev/null)

    # Extract Key ID and Secret key using the correct patterns from actual output
    # Format: "Key ID: GKxxxxxxx" and "Secret key: xxxxxxx"
    local access_key=$(echo "$key_output" | grep "Key ID:" | awk '{print $3}')
    local secret_key=$(echo "$key_output" | grep "Secret key:" | awk '{print $3}')

    if [[ -z "$access_key" || -z "$secret_key" ]]; then
        log_error "Failed to extract keys from output" >&2
        log_error "Expected format: 'Key ID: GKxxxxxxx' and 'Secret key: xxxxxxx'" >&2
        echo "Actual output:" >&2
        echo "$key_output" >&2
        exit 1
    fi

    log_success "Key '$KEY_NAME' created" >&2
    log_info "Access Key ID: $access_key" >&2
    log_info "Secret Key: ${secret_key:0:10}..." >&2 # Only show first 10 chars for security

    # Store keys for later use
    export GARAGE_ACCESS_KEY="$access_key"
    export GARAGE_SECRET_KEY="$secret_key"

    # Only output the keys to stdout for capture
    echo "$access_key $secret_key"
}

# Function to set bucket permissions
set_bucket_permissions() {
    log_info "Setting bucket permissions..."
    garage_exec bucket allow \
        --read \
        --write \
        --owner \
        "$BUCKET_NAME" \
        --key "$KEY_NAME"

    log_success "Bucket permissions set"
}

# Function to create AWS config file (legacy format)
create_aws_config() {
    local access_key="$1"
    local secret_key="$2"

    log_info "Creating AWS configuration file at $AWS_RC_FILE..."

    cat > "$AWS_RC_FILE" << EOF
# Garage S3 AWS Configuration
export AWS_ACCESS_KEY_ID=$access_key
export AWS_SECRET_ACCESS_KEY=$secret_key
export AWS_DEFAULT_REGION='garage'
export AWS_ENDPOINT_URL='http://$HOST:$PORT'
EOF

    log_success "AWS configuration file created at $AWS_RC_FILE"
    log_info "To use these settings, run: source $AWS_RC_FILE"
}

# Function to create AWS credentials file (modern format)
create_aws_credentials() {
    local access_key="$1"
    local secret_key="$2"

    log_info "Creating AWS credentials profile '$PROFILE_NAME'..."

    mkdir -p "$AWS_DIR"

    # Remove existing profile if it exists to ensure fresh credentials
    if grep -q "\[$PROFILE_NAME\]" "$CREDENTIALS_FILE" 2>/dev/null; then
        log_info "Updating existing profile '$PROFILE_NAME' in credentials file..."
        # Create a temporary file without the existing profile
        local temp_file=$(mktemp)
        awk -v profile="$PROFILE_NAME" '
        BEGIN { skip = 0 }
        /^\[/ { 
            if ($0 == "[" profile "]") {
                skip = 1
            } else {
                skip = 0
            }
        }
        !skip { print }
        ' "$CREDENTIALS_FILE" > "$temp_file" 2>/dev/null || true
        mv "$temp_file" "$CREDENTIALS_FILE"
    fi

    # Add the new profile
    log_info "Writing credentials to $CREDENTIALS_FILE"
    {
        echo "[$PROFILE_NAME]"
        echo "aws_access_key_id = $access_key"
        echo "aws_secret_access_key = $secret_key"
        echo "region = garage"
    } >> "$CREDENTIALS_FILE"
    log_success "AWS credentials profile '$PROFILE_NAME' created/updated"
}

# Function to display status
show_status() {
    log_info "Displaying final status..."

    echo -e "\n${BLUE}=== BUCKET LIST ===${NC}"
    garage_exec bucket list

    echo -e "\n${BLUE}=== BUCKET INFO ===${NC}"
    garage_exec bucket info "$BUCKET_NAME"

    if command -v aws &> /dev/null; then
        echo -e "\n${BLUE}=== AWS CLI VERSION ===${NC}"
        aws --version

        echo -e "\n${BLUE}=== TESTING CONNECTION ===${NC}"
        log_info "Testing S3 connection with profile '$PROFILE_NAME'..."
        # Unset any conflicting AWS environment variables to ensure profile is used
        if (unset AWS_ACCESS_KEY_ID AWS_SECRET_ACCESS_KEY AWS_SESSION_TOKEN; AWS_PROFILE="$PROFILE_NAME" aws --endpoint-url "http://$HOST:$PORT" s3 ls) 2>/dev/null; then
            log_success "S3 connection test successful!"
        else
            log_warning "S3 connection test failed. Check your configuration."
        fi
    else
        log_warning "AWS CLI not found. Install it to test S3 connectivity."
    fi
}

# Help function
show_help() {
    echo "Garage S3 Container Setup Script"
    echo ""
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -h, --help              Show this help message"
    echo "  --reset                 Reset volume and restart with fresh data"
    echo "  -c, --container NAME    Container name (default: $CONTAINER_NAME)"
    echo "  -z, --zone ZONE         Zone name (default: $ZONE)"
    echo "  -s, --capacity SIZE     Storage capacity (default: $CAPACITY)"
    echo "  -b, --bucket NAME       Bucket name (default: $BUCKET_NAME)"
    echo "  -k, --key NAME          Key name (default: $KEY_NAME)"
    echo "  -p, --profile NAME      AWS profile name (default: $PROFILE_NAME)"
    echo "  --host HOST             Host address (default: $HOST)"
    echo "  --port PORT             Port number (default: $PORT)"
    echo ""
    echo "Examples:"
    echo "  $0                      # Run with default settings"
    echo "  $0 --reset              # Reset and start fresh"
    echo "  $0 -c my-garage -b my-bucket"
    echo "  $0 --zone dc2 --capacity 5G"
    echo "  $0 -p my-profile --host 192.168.1.100"
}
