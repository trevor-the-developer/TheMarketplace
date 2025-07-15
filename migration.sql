IF
OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
CREATE TABLE [__EFMigrationsHistory]
(
    [
    MigrationId]
    nvarchar
(
    150
) NOT NULL,
    [ProductVersion] nvarchar
(
    32
) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY
(
[
    MigrationId]
)
    );
END;
GO

BEGIN
TRANSACTION;
CREATE TABLE [AspNetRoles]
(
    [
    Id]
    nvarchar
(
    450
) NOT NULL,
    [Name] nvarchar
(
    256
) NULL,
    [NormalizedName] nvarchar
(
    256
) NULL,
    [ConcurrencyStamp] nvarchar
(
    max
) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY
(
[
    Id]
)
    );

CREATE TABLE [AspNetUsers]
(
    [
    Id]
    nvarchar
(
    450
) NOT NULL,
    [ApplicationUserId] int NOT NULL,
    [FirstName] nvarchar
(
    max
) NULL,
    [LastName] nvarchar
(
    max
) NULL,
    [DateOfBirth] datetime2 NULL,
    [Role] nvarchar
(
    max
) NOT NULL,
    [RefreshToken] nvarchar
(
    max
) NULL,
    [RefreshTokenExpiry] datetime2 NOT NULL,
    [UserName] nvarchar
(
    256
) NULL,
    [NormalizedUserName] nvarchar
(
    256
) NULL,
    [Email] nvarchar
(
    256
) NULL,
    [NormalizedEmail] nvarchar
(
    256
) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar
(
    max
) NULL,
    [SecurityStamp] nvarchar
(
    max
) NULL,
    [ConcurrencyStamp] nvarchar
(
    max
) NULL,
    [PhoneNumber] nvarchar
(
    max
) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY
(
[
    Id]
)
    );

CREATE TABLE [Listings]
(
    [
    Id]
    int
    NOT
    NULL
    IDENTITY, [
    Title]
    nvarchar
(
    max
) NOT NULL,
    [Description] nvarchar
(
    max
) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar
(
    max
) NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [ModifiedBy] nvarchar
(
    max
) NOT NULL,
    CONSTRAINT [PK_Listings] PRIMARY KEY
(
[
    Id]
)
    );

CREATE TABLE [Tags]
(
    [
    Id]
    int
    NOT
    NULL
    IDENTITY, [
    Name]
    nvarchar
(
    max
) NOT NULL,
    [Description] nvarchar
(
    max
) NULL,
    [IsEnabled] bit NULL,
    [TagId] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar
(
    max
) NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [ModifiedBy] nvarchar
(
    max
) NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY
(
[
    Id]
),
    CONSTRAINT [FK_Tags_Tags_TagId] FOREIGN KEY
(
[
    TagId]
) REFERENCES [Tags]
(
[
    Id]
)
    );

CREATE TABLE [AspNetRoleClaims]
(
    [
    Id]
    int
    NOT
    NULL
    IDENTITY, [
    RoleId]
    nvarchar
(
    450
) NOT NULL,
    [ClaimType] nvarchar
(
    max
) NULL,
    [ClaimValue] nvarchar
(
    max
) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY
(
[
    Id]
),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY
(
[
    RoleId]
) REFERENCES [AspNetRoles]
(
[
    Id]
) ON DELETE CASCADE
    );

CREATE TABLE [AspNetUserClaims]
(
    [
    Id]
    int
    NOT
    NULL
    IDENTITY, [
    UserId]
    nvarchar
(
    450
) NOT NULL,
    [ClaimType] nvarchar
(
    max
) NULL,
    [ClaimValue] nvarchar
(
    max
) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY
(
[
    Id]
),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY
(
[
    UserId]
) REFERENCES [AspNetUsers]
(
[
    Id]
) ON DELETE CASCADE
    );

CREATE TABLE [AspNetUserLogins]
(
    [
    LoginProvider]
    nvarchar
(
    450
) NOT NULL,
    [ProviderKey] nvarchar
(
    450
) NOT NULL,
    [ProviderDisplayName] nvarchar
(
    max
) NULL,
    [UserId] nvarchar
(
    450
) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY
(
    [
    LoginProvider],
[
    ProviderKey]
),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY
(
[
    UserId]
) REFERENCES [AspNetUsers]
(
[
    Id]
) ON DELETE CASCADE
    );

CREATE TABLE [AspNetUserRoles]
(
    [
    UserId]
    nvarchar
(
    450
) NOT NULL,
    [RoleId] nvarchar
(
    450
) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY
(
    [
    UserId],
[
    RoleId]
),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY
(
[
    RoleId]
) REFERENCES [AspNetRoles]
(
[
    Id]
) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY
(
[
    UserId]
) REFERENCES [AspNetUsers]
(
[
    Id]
)
  ON DELETE CASCADE
    );

CREATE TABLE [AspNetUserTokens]
(
    [
    UserId]
    nvarchar
(
    450
) NOT NULL,
    [LoginProvider] nvarchar
(
    450
) NOT NULL,
    [Name] nvarchar
(
    450
) NOT NULL,
    [Value] nvarchar
(
    max
) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY
(
    [
    UserId], [
    LoginProvider],
[
    Name]
),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY
(
[
    UserId]
) REFERENCES [AspNetUsers]
(
[
    Id]
) ON DELETE CASCADE
    );

CREATE TABLE [Profiles]
(
    [
    ApplicationUserId]
    nvarchar
(
    450
) NOT NULL,
    [DisplayName] nvarchar
(
    max
) NOT NULL,
    [Bio] nvarchar
(
    max
) NOT NULL,
    [SocialMedia] nvarchar
(
    max
) NOT NULL,
    [Id] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar
(
    max
) NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [ModifiedBy] nvarchar
(
    max
) NOT NULL,
    CONSTRAINT [PK_Profiles] PRIMARY KEY
(
[
    ApplicationUserId]
),
    CONSTRAINT [FK_Profiles_AspNetUsers_ApplicationUserId] FOREIGN KEY
(
[
    ApplicationUserId]
) REFERENCES [AspNetUsers]
(
[
    Id]
) ON DELETE CASCADE
    );

CREATE TABLE [Cards]
(
    [
    Id]
    int
    NOT
    NULL
    IDENTITY, [
    Title]
    nvarchar
(
    100
) NOT NULL,
    [Description] nvarchar
(
    500
) NULL,
    [IsEnabled] bit NULL,
    [Colour] nvarchar
(
    20
) NULL,
    [ListingId] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar
(
    max
) NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [ModifiedBy] nvarchar
(
    max
) NOT NULL,
    CONSTRAINT [PK_Cards] PRIMARY KEY
(
[
    Id]
),
    CONSTRAINT [FK_Cards_Listings_ListingId] FOREIGN KEY
(
[
    ListingId]
) REFERENCES [Listings]
(
[
    Id]
) ON DELETE CASCADE
    );

CREATE TABLE [Products]
(
    [
    Id]
    int
    NOT
    NULL
    IDENTITY, [
    Title]
    nvarchar
(
    100
) NOT NULL,
    [Description] nvarchar
(
    500
) NULL,
    [ProductType] nvarchar
(
    50
) NULL,
    [Category] nvarchar
(
    50
) NULL,
    [IsEnabled] bit NULL,
    [IsDeleted] bit NULL,
    [CardId] int NULL,
    [ProductDetailId] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar
(
    max
) NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [ModifiedBy] nvarchar
(
    max
) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY
(
[
    Id]
),
    CONSTRAINT [FK_Products_Cards_CardId] FOREIGN KEY
(
[
    CardId]
) REFERENCES [Cards]
(
[
    Id]
) ON DELETE NO ACTION
    );

CREATE TABLE [ProductDetails]
(
    [
    Id]
    int
    NOT
    NULL
    IDENTITY, [
    Title]
    nvarchar
(
    max
) NOT NULL,
    [Description] nvarchar
(
    max
) NULL,
    [ProductId] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar
(
    max
) NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [ModifiedBy] nvarchar
(
    max
) NOT NULL,
    CONSTRAINT [PK_ProductDetails] PRIMARY KEY
(
[
    Id]
),
    CONSTRAINT [FK_ProductDetails_Products_ProductId] FOREIGN KEY
(
[
    ProductId]
) REFERENCES [Products]
(
[
    Id]
)
    );

CREATE TABLE [Documents]
(
    [
    Id]
    int
    NOT
    NULL
    IDENTITY, [
    Title]
    nvarchar
(
    100
) NOT NULL,
    [Description] nvarchar
(
    500
) NULL,
    [Text] nvarchar
(
    max
) NULL,
    [Bytes] nvarchar
(
    max
) NULL,
    [DocumentType] nvarchar
(
    50
) NOT NULL,
    [ProductDetailId] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar
(
    max
) NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [ModifiedBy] nvarchar
(
    max
) NOT NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY
(
[
    Id]
),
    CONSTRAINT [FK_Documents_ProductDetails_ProductDetailId] FOREIGN KEY
(
[
    ProductDetailId]
) REFERENCES [ProductDetails]
(
[
    Id]
) ON DELETE CASCADE
    );

CREATE TABLE [Files]
(
    [
    Id]
    int
    NOT
    NULL
    IDENTITY, [
    Title]
    nvarchar
(
    max
) NOT NULL,
    [Description] nvarchar
(
    max
) NULL,
    [FilePath] nvarchar
(
    max
) NULL,
    [DirectoryPath] nvarchar
(
    max
) NULL,
    [MediaType] nvarchar
(
    max
) NULL,
    [ProductDetailId] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar
(
    max
) NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [ModifiedBy] nvarchar
(
    max
) NOT NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY
(
[
    Id]
),
    CONSTRAINT [FK_Files_ProductDetails_ProductDetailId] FOREIGN KEY
(
[
    ProductDetailId]
) REFERENCES [ProductDetails]
(
[
    Id]
) ON DELETE CASCADE
    );

IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] ON;
INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES (N'00917cdb-f5b0-4c84-9172-ff5b72ff8500', NULL, N'Administrator', N'ADMINISTRATOR'), (N'e23ba8c8-b3ae-4e81-b468-c269c6e35cf2', NULL, N'User', N'USER');
IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] OFF;

IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ApplicationUserId', N'ConcurrencyStamp', N'DateOfBirth', N'Email', N'EmailConfirmed', N'FirstName', N'LastName', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'RefreshToken', N'RefreshTokenExpiry', N'Role', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT [AspNetUsers] ON;
INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [ApplicationUserId], [ConcurrencyStamp], [DateOfBirth], [Email], [
                            EmailConfirmed], [FirstName], [LastName], [LockoutEnabled], [LockoutEnd], [
                            NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [
                            PhoneNumberConfirmed], [RefreshToken], [RefreshTokenExpiry], [Role], [SecurityStamp], [
                            TwoFactorEnabled], [UserName])
VALUES (N'69a38a69-e24d-4c7f-bdf2-c7bc2222cbe7', 0, 0, N'c610cc21-539b-497e-b662-ef096e4afe26', NULL, N'demouser@localhost', CAST (1 AS bit), N'Demo', N'User', CAST (0 AS bit), NULL, N'DEMOUSER@LOCALHOST', N'DEMOUSER@LOCALHOST', N'AQAAAAIAAYagAAAAEJnpDwsI/3+lVtRHhNefc2ZXBKLdQwRLOfWUkAI666Uf5q/oQmGjhYY7uDrZVYGGQw==', NULL, CAST (0 AS bit), NULL, '0001-01-01T00:00:00.0000000', N'User', N'9aa47184-f48a-47f6-b15d-086c46598a7c', CAST (0 AS bit), N'demouser@localhost'), (N'a5ac5ebb-5f11-4363-a58d-4362d8ff6863', 0, 0, N'd390510b-1631-42cd-af08-1c6184a51d45', NULL, N'admin@localhost', CAST (1 AS bit), N'System', N'Administrator', CAST (0 AS bit), NULL, N'ADMIN@LOCALHOST', N'ADMIN@LOCALHOST', N'AQAAAAIAAYagAAAAECkxmrPhjQdbUsekNI5uuJpeYDOOfo/0QkiMMNE+JxS2Qrro6QPvAWAEK6/b/hAo8Q==', NULL, CAST (0 AS bit), NULL, '0001-01-01T00:00:00.0000000', N'Adminstrator', N'3b882abd-b1ed-4f09-bc23-7e35e1e97b40', CAST (0 AS bit), N'admin@localhost');
IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ApplicationUserId', N'ConcurrencyStamp', N'DateOfBirth', N'Email', N'EmailConfirmed', N'FirstName', N'LastName', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'RefreshToken', N'RefreshTokenExpiry', N'Role', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT [AspNetUsers] OFF;

IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedBy', N'CreatedDate', N'Description', N'DirectoryPath', N'FilePath', N'MediaType', N'ModifiedBy', N'ModifiedDate', N'ProductDetailId', N'Title') AND [object_id] = OBJECT_ID(N'[Files]'))
    SET IDENTITY_INSERT [Files] ON;
INSERT INTO [Files] ([Id], [CreatedBy], [CreatedDate], [Description], [DirectoryPath], [FilePath], [MediaType], [
                      ModifiedBy], [ModifiedDate], [ProductDetailId], [Title])
VALUES (1, N'', '2024-05-29T19:06:31.3087911+01:00', N'This is a sample media file', N'media', N'sample.mp4', N'video', N'', '2024-05-29T19:06:31.3087964+01:00', NULL, N'Sample Media'), (2, N'', '2024-05-29T19:06:31.3087968+01:00', N'This is a sample media file', N'media', N'sample.mp4', N'video', N'', '2024-05-29T19:06:31.3087970+01:00', NULL, N'Sample Media'), (3, N'', '2024-05-29T19:06:31.3087973+01:00', N'This is a sample media file', N'media', N'sample.mp4', N'video', N'', '2024-05-29T19:06:31.3087974+01:00', NULL, N'Sample Media');
IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedBy', N'CreatedDate', N'Description', N'DirectoryPath', N'FilePath', N'MediaType', N'ModifiedBy', N'ModifiedDate', N'ProductDetailId', N'Title') AND [object_id] = OBJECT_ID(N'[Files]'))
    SET IDENTITY_INSERT [Files] OFF;

IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedBy', N'CreatedDate', N'Description', N'ModifiedBy', N'ModifiedDate', N'Title') AND [object_id] = OBJECT_ID(N'[Listings]'))
    SET IDENTITY_INSERT [Listings] ON;
INSERT INTO [Listings] ([Id], [CreatedBy], [CreatedDate], [Description], [ModifiedBy], [ModifiedDate], [Title])
VALUES (1, N'John Doe', '2024-05-29T19:06:31.3101578+01:00', NULL, N'John Doe', '2024-05-29T19:06:31.3101593+01:00', N'Sample Listing');
IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedBy', N'CreatedDate', N'Description', N'ModifiedBy', N'ModifiedDate', N'Title') AND [object_id] = OBJECT_ID(N'[Listings]'))
    SET IDENTITY_INSERT [Listings] OFF;

IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedBy', N'CreatedDate', N'Description', N'ModifiedBy', N'ModifiedDate', N'ProductId', N'Title') AND [object_id] = OBJECT_ID(N'[ProductDetails]'))
    SET IDENTITY_INSERT [ProductDetails] ON;
INSERT INTO [ProductDetails] ([Id], [CreatedBy], [CreatedDate], [Description], [ModifiedBy], [ModifiedDate], [
                               ProductId], [Title])
VALUES (30, N'John Doe', '2024-05-29T19:06:31.3091163+01:00', N'This is a sample product detail', N'John Doe', '2024-05-29T19:06:31.3091180+01:00', NULL, N'Sample Product Detail'), (31, N'Jane Smith', '2024-05-29T19:06:31.3091183+01:00', N'This is another sample product detail', N'Jane Smith', '2024-05-29T19:06:31.3091185+01:00', NULL, N'Another Sample Product Detail'), (32, N'John Doe', '2024-05-29T19:06:31.3091188+01:00', N'This is yet another sample product detail', N'John Doe', '2024-05-29T19:06:31.3091190+01:00', NULL, N'Yet Another Sample Product Detail');
IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedBy', N'CreatedDate', N'Description', N'ModifiedBy', N'ModifiedDate', N'ProductId', N'Title') AND [object_id] = OBJECT_ID(N'[ProductDetails]'))
    SET IDENTITY_INSERT [ProductDetails] OFF;

IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
    SET IDENTITY_INSERT [AspNetUserRoles] ON;
INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
VALUES (N'e23ba8c8-b3ae-4e81-b468-c269c6e35cf2', N'69a38a69-e24d-4c7f-bdf2-c7bc2222cbe7'), (N'00917cdb-f5b0-4c84-9172-ff5b72ff8500', N'a5ac5ebb-5f11-4363-a58d-4362d8ff6863');
IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
    SET IDENTITY_INSERT [AspNetUserRoles] OFF;

IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Colour', N'CreatedBy', N'CreatedDate', N'Description', N'IsEnabled', N'ListingId', N'ModifiedBy', N'ModifiedDate', N'Title') AND [object_id] = OBJECT_ID(N'[Cards]'))
    SET IDENTITY_INSERT [Cards] ON;
INSERT INTO [Cards] ([Id], [Colour], [CreatedBy], [CreatedDate], [Description], [IsEnabled], [ListingId], [
                      ModifiedBy], [ModifiedDate], [Title])
VALUES (1, N'Blue', N'', '0001-01-01T00:00:00.0000000', N'This is a sample card', CAST (1 AS bit), 1, N'', '0001-01-01T00:00:00.0000000', N'Sample Card'), (2, N'Red', N'', '0001-01-01T00:00:00.0000000', N'This is another card', CAST (0 AS bit), 1, N'', '0001-01-01T00:00:00.0000000', N'Another Card'), (3, N'Green', N'', '0001-01-01T00:00:00.0000000', N'This is the third card', CAST (1 AS bit), 1, N'', '0001-01-01T00:00:00.0000000', N'Third Card');
IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Colour', N'CreatedBy', N'CreatedDate', N'Description', N'IsEnabled', N'ListingId', N'ModifiedBy', N'ModifiedDate', N'Title') AND [object_id] = OBJECT_ID(N'[Cards]'))
    SET IDENTITY_INSERT [Cards] OFF;

IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CardId', N'Category', N'CreatedBy', N'CreatedDate', N'Description', N'IsDeleted', N'IsEnabled', N'ModifiedBy', N'ModifiedDate', N'ProductDetailId', N'ProductType', N'Title') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] ([Id], [CardId], [Category], [CreatedBy], [CreatedDate], [Description], [IsDeleted], [
                         IsEnabled], [ModifiedBy], [ModifiedDate], [ProductDetailId], [ProductType], [Title])
VALUES (1, 1, N'Sample Category', N'Sample User', '2024-05-29T19:06:31.3097484+01:00', N'This is a sample product', CAST (0 AS bit), CAST (1 AS bit), N'Sample User', '2024-05-29T19:06:31.3097500+01:00', 21, N'Sample Type', N'Sample Product'), (2, 1, N'Another Sample Category', N'Another Sample User', '2024-05-29T19:06:31.3097505+01:00', N'This is another sample product', CAST (0 AS bit), CAST (1 AS bit), N'Another Sample User', '2024-05-29T19:06:31.3097507+01:00', 22, N'Another Sample Type', N'Another Sample Product'), (3, 1, N'Third Sample Category', N'Third Sample User', '2024-05-29T19:06:31.3097511+01:00', N'This is the third sample product', CAST (0 AS bit), CAST (1 AS bit), N'Third Sample User', '2024-05-29T19:06:31.3097513+01:00', 23, N'Third Sample Type', N'Third Sample Product');
IF
EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CardId', N'Category', N'CreatedBy', N'CreatedDate', N'Description', N'IsDeleted', N'IsEnabled', N'ModifiedBy', N'ModifiedDate', N'ProductDetailId', N'ProductType', N'Title') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] OFF;

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_Cards_ListingId] ON [Cards] ([ListingId]);

CREATE INDEX [IX_Documents_ProductDetailId] ON [Documents] ([ProductDetailId]);

CREATE INDEX [IX_Files_ProductDetailId] ON [Files] ([ProductDetailId]);

CREATE UNIQUE INDEX [IX_ProductDetails_ProductId] ON [ProductDetails] ([ProductId]) WHERE [ProductId] IS NOT NULL;

CREATE INDEX [IX_Products_CardId] ON [Products] ([CardId]);

CREATE INDEX [IX_Tags_TagId] ON [Tags] ([TagId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240529180631_InitialCreate', N'9.0.0');

COMMIT;
GO

