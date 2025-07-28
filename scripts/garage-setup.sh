#!/bin/bash

# ========================================
#     Garage S3 Container Setup Script
# ========================================
# This script initializes a fresh Garage container with the required configuration

set -euo pipefail

source ./garage-setup-functions.sh

# Configuration variables
CONTAINER_NAME="garage"
CONFIG_PATH="/tmp/garage.toml"
ZONE="dc1"
CAPACITY="1G"
BUCKET_NAME="the-marketplace"
KEY_NAME="the-marketplace-key"
PROFILE_NAME="garage-local"
AWS_DIR="$HOME/.aws"
CREDENTIALS_FILE="$AWS_DIR/credentials"
AWS_RC_FILE="$HOME/.awsrc"
GARAGE_VOLUME="garage-data"
HOST="localhost"
PORT="3900"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color


# Main execution
main() {
    print_banner

    # Handle --reset flag
    if [[ "${1:-}" == "--reset" ]]; then
        reset_volume
        log_info "Starting Garage container with fresh volume..."
        docker compose up -d "$CONTAINER_NAME" 2>/dev/null || docker-compose up -d "$CONTAINER_NAME" 2>/dev/null
        log_info "Waiting for container to be ready..."
        sleep 5
        shift # Remove --reset from arguments
    fi

    # Ensure container is running
    log_info "Ensuring container '$CONTAINER_NAME' is running..."
    if ! docker ps --format "table {{.Names}}" | grep -q "^${CONTAINER_NAME}$"; then
        log_info "Starting container '$CONTAINER_NAME'..."
        docker compose up -d "$CONTAINER_NAME" 2>/dev/null || docker-compose up -d "$CONTAINER_NAME" 2>/dev/null
        log_info "Waiting for container to be ready..."
        sleep 5
    fi

    # Check if container is running
    check_container

    # Get node ID directly to avoid function call issues
    log_info "Getting garage node ID..."
    local node_id=$(garage_exec status 2>/dev/null | awk '/^==== HEALTHY NODES ====/ { getline; getline; print $1 }')

    if [[ -z "$node_id" ]]; then
        log_error "Could not extract node ID from 'garage status'"
        garage_exec status
        exit 1
    fi

    log_success "Found node ID: $node_id"

    # Setup layout
    setup_layout "$node_id"

    # Create bucket
    create_bucket

    # Create key and capture credentials
    local key_output=($(create_key))
    local access_key="${key_output[0]}"
    local secret_key="${key_output[1]:-}"  # Use empty string if not available

    # Set bucket permissions if we have an access key
    if [[ -n "$access_key" ]]; then
        set_bucket_permissions

        # Create AWS config files only if we have both keys
        if [[ -n "$secret_key" ]]; then
            # Create both AWS config formats
            create_aws_config "$access_key" "$secret_key"
            create_aws_credentials "$access_key" "$secret_key"
        else
            log_warning "Secret key not available. Skipping AWS config file creation."
            log_info "If this key already existed, you'll need to use your existing secret key."
        fi
    else
        log_error "No access key available. Cannot set permissions or create config files."
        exit 1
    fi
    
    # If the key already exists, try to grab the secret from ~/.aws/credentials
    if [[ -z "$secret_key" ]]; then
        secret_key=$(awk -v p="[$PROFILE_NAME]" '
            $0==p {getline; if($1=="aws_secret_access_key") {print $3}}
        ' "$CREDENTIALS_FILE" 2>/dev/null)
    fi

    # Show final status
    show_status

    echo -e "\n${GREEN}========================================${NC}"
    echo -e "${GREEN}         Setup Complete!                ${NC}"
    echo -e "${GREEN}========================================${NC}"
    echo -e "${YELLOW}Next steps:${NC}"
    echo -e "1. Use AWS Profile: ${BLUE}AWS_PROFILE=$PROFILE_NAME aws --endpoint-url http://$HOST:$PORT s3 ls${NC}"
    echo -e "2. Or source the config: ${BLUE}source $AWS_RC_FILE && aws s3 ls${NC}"
    echo -e "3. Upload a file: ${BLUE}AWS_PROFILE=$PROFILE_NAME aws --endpoint-url http://$HOST:$PORT s3 cp file.txt s3://$BUCKET_NAME/${NC}"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            show_help
            exit 0
            ;;
        --reset)
            # Handle reset flag in main function
            break
            ;;
        -c|--container)
            CONTAINER_NAME="$2"
            shift 2
            ;;
        -z|--zone)
            ZONE="$2"
            shift 2
            ;;
        -s|--capacity)
            CAPACITY="$2"
            shift 2
            ;;
        -b|--bucket)
            BUCKET_NAME="$2"
            shift 2
            ;;
        -k|--key)
            KEY_NAME="$2"
            shift 2
            ;;
        -p|--profile)
            PROFILE_NAME="$2"
            shift 2
            ;;
        --host)
            HOST="$2"
            shift 2
            ;;
        --port)
            PORT="$2"
            shift 2
            ;;
        *)
            log_error "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# Run main function with all arguments
main "$@"
