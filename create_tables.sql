CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `Companies` (
    `Id` char(36) NOT NULL,
    `Name` varchar(255) NOT NULL,
    `Industry` varchar(150) NOT NULL,
    `Description` longtext NOT NULL,
    `Website` varchar(500) NULL,
    `Email` varchar(255) NOT NULL,
    `PhoneNumber` varchar(20) NOT NULL,
    `Logo` varchar(500) NULL,
    `IsVerified` tinyint(1) NOT NULL,
    `VerifiedAt` datetime(6) NULL,
    `FoundedYear` int NULL,
    `CompanySize` longtext NOT NULL,
    `CompanyType` longtext NOT NULL,
    `Headquarters` longtext NULL,
    `LogoUrl` longtext NULL,
    `LogoPublicId` longtext NULL,
    `LinkedInUrl` longtext NULL,
    `TwitterUrl` longtext NULL,
    `FacebookUrl` longtext NULL,
    `InstagramUrl` longtext NULL,
    `InvitationCode` longtext NULL,
    `InvitationCodeExpiry` datetime(6) NULL,
    `Locations` longtext NULL,
    `Strengths` longtext NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Conversation` (
    `Id` char(36) NOT NULL,
    `Title` longtext NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `DateModified` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `JobCategory` (
    `Id` char(36) NOT NULL,
    `Name` varchar(100) NOT NULL,
    `Description` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Skill` (
    `Id` char(36) NOT NULL,
    `Name` varchar(100) NOT NULL,
    `Description` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Users` (
    `Id` varchar(255) NOT NULL,
    `FirstName` varchar(100) NOT NULL,
    `LastName` varchar(100) NOT NULL,
    `UserName` longtext NOT NULL,
    `Email` varchar(255) NOT NULL,
    `HashedPassword` varchar(500) NOT NULL,
    `Tel` longtext NULL,
    `Role` varchar(50) NOT NULL,
    `ProfilePictureUrl` varchar(500) NULL,
    `ProfilePicturePublicId` longtext NULL,
    `Bio` longtext NOT NULL,
    `Location` varchar(200) NULL,
    `IsVerified` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `VerificationToken` longtext NULL,
    `VerificationTokenExpiry` datetime(6) NULL,
    `PasswordResetToken` longtext NULL,
    `PasswordResetTokenExpiry` datetime(6) NULL,
    `RefreshToken` longtext NULL,
    `RefreshTokenExpiry` datetime(6) NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `DateModified` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `AuditLog` (
    `Id` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `Action` varchar(200) NOT NULL,
    `Description` longtext NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `Timestamp` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AuditLog_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ConversationParticipant` (
    `Id` char(36) NOT NULL,
    `ConversationId` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `JoinedAt` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ConversationParticipant_Conversation_ConversationId` FOREIGN KEY (`ConversationId`) REFERENCES `Conversation` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ConversationParticipant_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `FileUpload` (
    `Id` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `FileName` varchar(255) NOT NULL,
    `FileUrl` varchar(500) NOT NULL,
    `FileType` varchar(100) NOT NULL,
    `FileSize` int NOT NULL,
    `UploadedOn` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_FileUpload_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Message` (
    `Id` char(36) NOT NULL,
    `ConversationId` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `Content` longtext NOT NULL,
    `IsRead` tinyint(1) NOT NULL DEFAULT FALSE,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Message_Conversation_ConversationId` FOREIGN KEY (`ConversationId`) REFERENCES `Conversation` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Message_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Notification` (
    `Id` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `Title` varchar(200) NOT NULL,
    `Message` longtext NOT NULL,
    `Type` int NOT NULL,
    `IsRead` tinyint(1) NOT NULL DEFAULT FALSE,
    `Status` int NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Notification_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Post` (
    `Id` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `Content` longtext NOT NULL,
    `PostContenetUrl` varchar(500) NOT NULL,
    `Visibility` int NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `DateUpdated` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Post_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ProfessionalProfiles` (
    `Id` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `HeadLine` varchar(255) NULL,
    `Summary` longtext NULL,
    `GitHubUrl` varchar(500) NULL,
    `LinkedInUrl` varchar(500) NULL,
    `ResumeUrl` varchar(500) NULL,
    `ResumePublicId` varchar(255) NULL,
    `ResumeViewCount` int NOT NULL,
    `ResumeDownloadCount` int NOT NULL,
    `UserStatus` varchar(100) NOT NULL,
    `PreferredJobTypes` longtext NOT NULL,
    `PreferredLocations` longtext NOT NULL,
    `EarliestStartDate` datetime(6) NULL,
    `WillingToRelocate` tinyint(1) NOT NULL,
    `WorkAuthorization` varchar(100) NOT NULL,
    `AvailabilityVisibility` varchar(100) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `DateModified` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProfessionalProfiles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `RecruiterProfiles` (
    `Id` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `CompanyId` char(36) NULL,
    `JobTitle` varchar(150) NULL,
    `IsCompanyAdmin` tinyint(1) NOT NULL DEFAULT FALSE,
    `Department` varchar(150) NULL,
    `Status` int NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `DateModified` datetime(6) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_RecruiterProfiles_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_RecruiterProfiles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `UserConnection` (
    `Id` char(36) NOT NULL,
    `SenderId` varchar(255) NOT NULL,
    `RecieverId` varchar(255) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `ConnectionStatus` int NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `DateUpdated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_UserConnection_Users_RecieverId` FOREIGN KEY (`RecieverId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_UserConnection_Users_SenderId` FOREIGN KEY (`SenderId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Comment` (
    `Id` char(36) NOT NULL,
    `PostId` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `Content` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateUpdated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Comment_Post_PostId` FOREIGN KEY (`PostId`) REFERENCES `Post` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Comment_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `PostLike` (
    `Id` char(36) NOT NULL,
    `PostId` char(36) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PostLike_Post_PostId` FOREIGN KEY (`PostId`) REFERENCES `Post` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_PostLike_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Report` (
    `Id` char(36) NOT NULL,
    `ReporterId` varchar(255) NOT NULL,
    `ReportedUserId` varchar(255) NOT NULL,
    `PostId` char(36) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `Reason` int NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `ReportStatus` int NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Report_Post_PostId` FOREIGN KEY (`PostId`) REFERENCES `Post` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Report_Users_ReportedUserId` FOREIGN KEY (`ReportedUserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Report_Users_ReporterId` FOREIGN KEY (`ReporterId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Certificate` (
    `Id` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `Name` varchar(200) NOT NULL,
    `IssuingOrganization` varchar(200) NOT NULL,
    `IssueDate` datetime(6) NOT NULL,
    `ExpireDate` datetime(6) NULL,
    `CredentialId` varchar(150) NOT NULL,
    `CredentialUrl` varchar(500) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Certificate_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Education` (
    `Id` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `Institution` varchar(200) NOT NULL,
    `Degree` varchar(150) NOT NULL,
    `FieldOfStudy` varchar(150) NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NULL,
    `Grade` varchar(50) NOT NULL,
    `Description` longtext NULL,
    `DateCreated` datetime(6) NOT NULL,
    `DateModified` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Education_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Experience` (
    `Id` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `CompanyName` varchar(200) NOT NULL,
    `JobTitle` varchar(150) NOT NULL,
    `EmploymentType` varchar(100) NOT NULL,
    `Location` varchar(150) NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `IsCurrentJob` tinyint(1) NOT NULL DEFAULT FALSE,
    `Description` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Experience_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `PortfolioLinks` (
    `Id` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `Title` varchar(200) NOT NULL,
    `Url` varchar(500) NOT NULL,
    `LinkType` varchar(50) NOT NULL,
    `Description` varchar(500) NULL,
    `ThumbnailUrl` varchar(500) NULL,
    `ThumbnailPublicId` varchar(255) NULL,
    `ViewCount` int NOT NULL,
    `ClickCount` int NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PortfolioLinks_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ProfessionalSkill` (
    `Id` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `SkillId` char(36) NOT NULL,
    `Level` varchar(50) NOT NULL,
    `YearsOfExperience` int NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProfessionalSkill_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ProfessionalSkill_Skill_SkillId` FOREIGN KEY (`SkillId`) REFERENCES `Skill` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Project` (
    `Id` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `Title` varchar(200) NOT NULL,
    `Description` longtext NOT NULL,
    `ProjectUrl` varchar(500) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Project_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Job` (
    `Id` char(36) NOT NULL,
    `CompanyId` char(36) NOT NULL,
    `RecruiterProfileId` char(36) NOT NULL,
    `JobCategoryId` char(36) NOT NULL,
    `Title` varchar(200) NOT NULL,
    `Description` longtext NOT NULL,
    `Requirement` longtext NOT NULL,
    `EmploymentType` varchar(100) NOT NULL,
    `WorkPlaceType` varchar(100) NOT NULL,
    `ExperienceLevel` varchar(100) NOT NULL,
    `MinSalary` decimal(18,2) NOT NULL,
    `MaxSalary` decimal(18,2) NOT NULL,
    `Currency` varchar(10) NOT NULL,
    `Location` varchar(150) NOT NULL,
    `ApplicationDeadline` datetime(6) NOT NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Job_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Job_JobCategory_JobCategoryId` FOREIGN KEY (`JobCategoryId`) REFERENCES `JobCategory` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Job_RecruiterProfiles_RecruiterProfileId` FOREIGN KEY (`RecruiterProfileId`) REFERENCES `RecruiterProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `JobApplication` (
    `Id` char(36) NOT NULL,
    `JobId` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `CoverLetter` longtext NOT NULL,
    `ResumeUrl` varchar(500) NULL,
    `ResumePublicId` longtext NULL,
    `JobStatus` int NOT NULL,
    `AppliedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_JobApplication_Job_JobId` FOREIGN KEY (`JobId`) REFERENCES `Job` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_JobApplication_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `SavedJob` (
    `Id` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `JobId` char(36) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `SavedAt` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SavedJob_Job_JobId` FOREIGN KEY (`JobId`) REFERENCES `Job` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_SavedJob_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

INSERT INTO `Conversation` (`Id`, `CreatedBy`, `DateCreated`, `DateModified`, `IsDeleted`, `Title`)
VALUES ('b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', '2026-08-13 05:47:49.656494', '0001-01-01 00:00:00.000000', FALSE, 'Soulshelf Group Chat');
SELECT ROW_COUNT();


INSERT INTO `Users` (`Id`, `Bio`, `CreatedBy`, `DateCreated`, `DateModified`, `Email`, `FirstName`, `HashedPassword`, `IsActive`, `IsDeleted`, `IsVerified`, `LastName`, `Location`, `PasswordResetToken`, `PasswordResetTokenExpiry`, `ProfilePicturePublicId`, `ProfilePictureUrl`, `RefreshToken`, `RefreshTokenExpiry`, `Role`, `Tel`, `UserName`, `VerificationToken`, `VerificationTokenExpiry`)
VALUES ('c117635d-96e0-409b-9fae-72976ec9c42a', 'I am the administrator of this platform.', 'system', '2026-08-13 05:47:49.575845', '0001-01-01 00:00:00.000000', 'admin@gmail.com', 'Ajibike', 'AQAAAAIAAYagAAAAEPNEImM+64fT1vZnS2O3YRM05aOKeFuso+Tazydf8qTEq/kI4mtTRnv1FmeDuWthnA==', TRUE, FALSE, TRUE, 'Abdussomad', 'Ogun State, Nigeria', NULL, NULL, NULL, NULL, NULL, NULL, 'admin', NULL, 'Administrator', NULL, NULL);
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `JoinedAt`, `UserId`)
VALUES ('21a6dfbc-5c10-4c45-8c1c-56d5adfd252c', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, '2026-08-13 05:47:49.657470', 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


CREATE INDEX `IX_AuditLog_UserId` ON `AuditLog` (`UserId`);

CREATE INDEX `IX_Certificate_ProfessionalProfileId` ON `Certificate` (`ProfessionalProfileId`);

CREATE INDEX `IX_Comment_PostId` ON `Comment` (`PostId`);

CREATE INDEX `IX_Comment_UserId` ON `Comment` (`UserId`);

CREATE UNIQUE INDEX `IX_ConversationParticipant_ConversationId_UserId` ON `ConversationParticipant` (`ConversationId`, `UserId`);

CREATE INDEX `IX_ConversationParticipant_UserId` ON `ConversationParticipant` (`UserId`);

CREATE INDEX `IX_Education_ProfessionalProfileId` ON `Education` (`ProfessionalProfileId`);

CREATE INDEX `IX_Experience_ProfessionalProfileId` ON `Experience` (`ProfessionalProfileId`);

CREATE INDEX `IX_FileUpload_UserId` ON `FileUpload` (`UserId`);

CREATE INDEX `IX_Job_CompanyId` ON `Job` (`CompanyId`);

CREATE INDEX `IX_Job_JobCategoryId` ON `Job` (`JobCategoryId`);

CREATE INDEX `IX_Job_RecruiterProfileId` ON `Job` (`RecruiterProfileId`);

CREATE UNIQUE INDEX `IX_JobApplication_JobId_ProfessionalProfileId` ON `JobApplication` (`JobId`, `ProfessionalProfileId`);

CREATE INDEX `IX_JobApplication_ProfessionalProfileId` ON `JobApplication` (`ProfessionalProfileId`);

CREATE UNIQUE INDEX `IX_JobCategory_Name` ON `JobCategory` (`Name`);

CREATE INDEX `IX_Message_ConversationId` ON `Message` (`ConversationId`);

CREATE INDEX `IX_Message_UserId` ON `Message` (`UserId`);

CREATE INDEX `IX_Notification_UserId` ON `Notification` (`UserId`);

CREATE INDEX `IX_PortfolioLinks_ProfessionalProfileId` ON `PortfolioLinks` (`ProfessionalProfileId`);

CREATE INDEX `IX_Post_UserId` ON `Post` (`UserId`);

CREATE UNIQUE INDEX `IX_PostLike_PostId_UserId` ON `PostLike` (`PostId`, `UserId`);

CREATE INDEX `IX_PostLike_UserId` ON `PostLike` (`UserId`);

CREATE UNIQUE INDEX `IX_ProfessionalProfiles_UserId` ON `ProfessionalProfiles` (`UserId`);

CREATE UNIQUE INDEX `IX_ProfessionalSkill_ProfessionalProfileId_SkillId` ON `ProfessionalSkill` (`ProfessionalProfileId`, `SkillId`);

CREATE INDEX `IX_ProfessionalSkill_SkillId` ON `ProfessionalSkill` (`SkillId`);

CREATE INDEX `IX_Project_ProfessionalProfileId` ON `Project` (`ProfessionalProfileId`);

CREATE INDEX `IX_RecruiterProfiles_CompanyId` ON `RecruiterProfiles` (`CompanyId`);

CREATE UNIQUE INDEX `IX_RecruiterProfiles_UserId` ON `RecruiterProfiles` (`UserId`);

CREATE INDEX `IX_Report_PostId` ON `Report` (`PostId`);

CREATE INDEX `IX_Report_ReportedUserId` ON `Report` (`ReportedUserId`);

CREATE INDEX `IX_Report_ReporterId` ON `Report` (`ReporterId`);

CREATE INDEX `IX_SavedJob_JobId` ON `SavedJob` (`JobId`);

CREATE UNIQUE INDEX `IX_SavedJob_ProfessionalProfileId_JobId` ON `SavedJob` (`ProfessionalProfileId`, `JobId`);

CREATE UNIQUE INDEX `IX_Skill_Name` ON `Skill` (`Name`);

CREATE INDEX `IX_UserConnection_RecieverId` ON `UserConnection` (`RecieverId`);

CREATE UNIQUE INDEX `IX_UserConnection_SenderId_RecieverId` ON `UserConnection` (`SenderId`, `RecieverId`);

CREATE UNIQUE INDEX `IX_Users_Email` ON `Users` (`Email`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260813054751_ChangesDueToRecruiterModule', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = '21a6dfbc-5c10-4c45-8c1c-56d5adfd252c';
SELECT ROW_COUNT();


ALTER TABLE `ProfessionalProfiles` ADD `AvailabilityStatus` varchar(50) NOT NULL DEFAULT '';

ALTER TABLE `ProfessionalProfiles` ADD `WebsiteUrl` varchar(500) NULL;

UPDATE `Conversation` SET `DateCreated` = '2026-08-15 19:26:08.603190'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `JoinedAt`, `UserId`)
VALUES ('1bc84b7e-febe-4ce5-a6c6-e90fcabf5bb7', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, '2026-08-15 19:26:08.604192', 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-08-15 19:26:08.525477', `HashedPassword` = 'AQAAAAIAAYagAAAAEI1C7QcfRLyQjxaEFtgnBozKN/jF1WAFOhbizSKOqnBUIgNQKoVE5ug86M2Wh+jX/g=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260815192610_UpdateAgain', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = '1bc84b7e-febe-4ce5-a6c6-e90fcabf5bb7';
SELECT ROW_COUNT();


ALTER TABLE `ProfessionalProfiles` ADD `ResumeFileName` longtext NULL;

ALTER TABLE `ProfessionalProfiles` ADD `ResumeFileSizeBytes` bigint NULL;

ALTER TABLE `ProfessionalProfiles` ADD `ResumeUploadedAt` datetime(6) NULL;

UPDATE `Conversation` SET `DateCreated` = '2026-08-17 16:29:25.148321'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `JoinedAt`, `UserId`)
VALUES ('82f0620f-aa87-4b89-a74b-8547f2ec174a', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, '2026-08-17 16:29:25.149334', 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-08-17 16:29:25.067494', `HashedPassword` = 'AQAAAAIAAYagAAAAEBkOrxwU7w3CXHH54Ilsmm3+/6Dt0/64GHrWSP5SDOFcrrBSM43NJCjG8RLs+1IFSw=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260817162926_NewMigration', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = '82f0620f-aa87-4b89-a74b-8547f2ec174a';
SELECT ROW_COUNT();


ALTER TABLE `Experience` ADD `CompanyId` char(36) NULL;

CREATE TABLE `UserFollow` (
    `Id` char(36) NOT NULL,
    `FollowerId` varchar(255) NOT NULL,
    `FollowingId` varchar(255) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `DateUpdated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_UserFollow_Users_FollowerId` FOREIGN KEY (`FollowerId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_UserFollow_Users_FollowingId` FOREIGN KEY (`FollowingId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

UPDATE `Conversation` SET `DateCreated` = '2026-08-19 19:55:33.058042'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `JoinedAt`, `UserId`)
VALUES ('d20b0dbc-3329-4813-be4d-eca3f1cc455f', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, '2026-08-19 19:55:33.059075', 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-08-19 19:55:32.979332', `HashedPassword` = 'AQAAAAIAAYagAAAAEEHVRxXbWiglFrdXffIbMP5bOTIKqQ3AVhLF5FlpotsodgcoYr7x0udaSKv2P5FMWQ=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE INDEX `IX_Experience_CompanyId` ON `Experience` (`CompanyId`);

CREATE UNIQUE INDEX `IX_UserFollow_FollowerId_FollowingId` ON `UserFollow` (`FollowerId`, `FollowingId`);

CREATE INDEX `IX_UserFollow_FollowingId` ON `UserFollow` (`FollowingId`);

ALTER TABLE `Experience` ADD CONSTRAINT `FK_Experience_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260819195536_NetworkingModuleFirstChanges', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = 'd20b0dbc-3329-4813-be4d-eca3f1cc455f';
SELECT ROW_COUNT();


ALTER TABLE `PostLike` ADD `ReactionType` int NOT NULL DEFAULT 0;

ALTER TABLE `Post` ADD `OriginalPostId` char(36) NULL;

ALTER TABLE `FileUpload` ADD `DisplayOrder` int NOT NULL DEFAULT 0;

ALTER TABLE `FileUpload` ADD `PostId` char(36) NULL;

UPDATE `Conversation` SET `DateCreated` = '2026-08-23 18:46:41.535747'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `JoinedAt`, `UserId`)
VALUES ('afc1cf0e-7d0b-4ea6-95a5-490fa91453cc', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, '2026-08-23 18:46:41.537245', 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-08-23 18:46:41.455777', `HashedPassword` = 'AQAAAAIAAYagAAAAEAoJdrDbOpWBkWeccVtA6gz9vSeChXj5bzGuu/ZniuFjxcmqO8WGZSTnWPKXUs1xUw=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE INDEX `IX_Post_OriginalPostId` ON `Post` (`OriginalPostId`);

CREATE INDEX `IX_FileUpload_PostId` ON `FileUpload` (`PostId`);

ALTER TABLE `FileUpload` ADD CONSTRAINT `FK_FileUpload_Post_PostId` FOREIGN KEY (`PostId`) REFERENCES `Post` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Post` ADD CONSTRAINT `FK_Post_Post_OriginalPostId` FOREIGN KEY (`OriginalPostId`) REFERENCES `Post` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260823184642_Init', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = 'afc1cf0e-7d0b-4ea6-95a5-490fa91453cc';
SELECT ROW_COUNT();


ALTER TABLE `Post` DROP COLUMN `PostContenetUrl`;

UPDATE `Conversation` SET `DateCreated` = '2026-08-24 00:38:33.949321'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `JoinedAt`, `UserId`)
VALUES ('6e9f0211-3a81-4f3d-a390-a4b2d94ca21c', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, '2026-08-24 00:38:33.950344', 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-08-24 00:38:33.864095', `HashedPassword` = 'AQAAAAIAAYagAAAAENnDfOWjVHRkifUIGFY4T70qoNENWotQivtERgwDDPkKiCMD0gaEowdovqbw/vTOGA=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260824003835_UpdatePostEntity', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = '6e9f0211-3a81-4f3d-a390-a4b2d94ca21c';
SELECT ROW_COUNT();


ALTER TABLE `Message` DROP COLUMN `IsRead`;

ALTER TABLE `Message` MODIFY `Content` longtext NULL;

ALTER TABLE `FileUpload` ADD `MessageId` char(36) NULL;

ALTER TABLE `ConversationParticipant` ADD `LastReadAt` datetime(6) NULL;

ALTER TABLE `Conversation` MODIFY `Title` longtext NULL;

ALTER TABLE `Conversation` ADD `GroupPhotoUrl` longtext NULL;

ALTER TABLE `Conversation` ADD `IsGroup` tinyint(1) NOT NULL DEFAULT FALSE;

UPDATE `Conversation` SET `DateCreated` = '2026-08-26 06:51:46.861216', `GroupPhotoUrl` = NULL, `IsGroup` = FALSE
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('f5a2280a-fa4f-4326-8ea6-23cfe041c270', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, '2026-08-26 06:51:46.862400', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-08-26 06:51:46.781419', `HashedPassword` = 'AQAAAAIAAYagAAAAEHEb8ziOux8oJQ/C04XrPVLgKNT6kS5WE7eMjLjAQ4+xiE9c5KFQR3OML7A0ldbE/A=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE INDEX `IX_FileUpload_MessageId` ON `FileUpload` (`MessageId`);

ALTER TABLE `FileUpload` ADD CONSTRAINT `FK_FileUpload_Message_MessageId` FOREIGN KEY (`MessageId`) REFERENCES `Message` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260826065148_ChangesDueToModuleSix', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = 'f5a2280a-fa4f-4326-8ea6-23cfe041c270';
SELECT ROW_COUNT();


ALTER TABLE `ConversationParticipant` ADD `IsHidden` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `ConversationParticipant` ADD `IsMuted` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `ConversationParticipant` ADD `IsPinned` tinyint(1) NOT NULL DEFAULT FALSE;

UPDATE `Conversation` SET `DateCreated` = '2026-08-27 07:10:15.524527'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('d11d07d1-d188-4555-af1f-35c75efd9516', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-08-27 07:10:15.525566', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-08-27 07:10:15.437064', `HashedPassword` = 'AQAAAAIAAYagAAAAEGK4OZb3ScfdKCGJCje9esRPNXE3WJppQAqcegUmTw5Gkwdj5H/l0w1FUaFWPu+85g=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260827071017_Remake', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = 'd11d07d1-d188-4555-af1f-35c75efd9516';
SELECT ROW_COUNT();


ALTER TABLE `JobApplication` ADD `AdditionalAnswers` longtext NULL;

ALTER TABLE `JobApplication` ADD `WorkAuthorization` varchar(50) NOT NULL DEFAULT '';

ALTER TABLE `Job` ADD `ScheduledPublishAt` datetime(6) NULL;

ALTER TABLE `Job` ADD `Status` int NOT NULL DEFAULT 0;

UPDATE `Conversation` SET `DateCreated` = '2026-08-30 09:27:29.047175'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('a0cb7a15-9d72-4159-902f-cd4332f0cf5b', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-08-30 09:27:29.048149', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-08-30 09:27:28.966496', `HashedPassword` = 'AQAAAAIAAYagAAAAEAhYg3YCSRrrvvM9OW3mot62ykpE/rLJybl2ketNaREz0zC7RHZRJ8OmpzeHaXkr2A=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260830092730_UpdatedModuleSeven', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = 'a0cb7a15-9d72-4159-902f-cd4332f0cf5b';
SELECT ROW_COUNT();


ALTER TABLE `JobApplication` ADD `InterviewLocationOrLink` longtext NULL;

ALTER TABLE `JobApplication` ADD `InterviewScheduledAt` datetime(6) NULL;

ALTER TABLE `JobApplication` ADD `InterviewType` longtext NULL;

UPDATE `Conversation` SET `DateCreated` = '2026-08-31 11:00:59.741224'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('e5a5939a-347c-4179-babe-8cfc6c99bbe8', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-08-31 11:00:59.742165', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-08-31 11:00:59.664357', `HashedPassword` = 'AQAAAAIAAYagAAAAEPBGRRTWCMRpabGL+yCK6g9QsDlhNOgf9CydtDF40eUIBycPV1mbAU4DOcHSbcjc3g=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260831110101_ReUpdateModuleSeven', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = 'e5a5939a-347c-4179-babe-8cfc6c99bbe8';
SELECT ROW_COUNT();


ALTER TABLE `JobApplication` ADD `ApplicantCurrentJobTitle` longtext NULL;

ALTER TABLE `JobApplication` ADD `ApplicantLinkedInUrl` longtext NULL;

ALTER TABLE `JobApplication` ADD `ApplicantLocation` longtext NULL;

ALTER TABLE `JobApplication` ADD `ApplicantPhone` longtext NULL;

ALTER TABLE `JobApplication` ADD `ApplicantYearsOfExperience` int NULL;

CREATE TABLE `JobSkill` (
    `Id` char(36) NOT NULL,
    `JobId` char(36) NOT NULL,
    `SkillId` char(36) NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_JobSkill_Job_JobId` FOREIGN KEY (`JobId`) REFERENCES `Job` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_JobSkill_Skill_SkillId` FOREIGN KEY (`SkillId`) REFERENCES `Skill` (`Id`) ON DELETE CASCADE
);

UPDATE `Conversation` SET `DateCreated` = '2026-09-02 07:39:38.617145'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('06035c74-acbd-42e2-924d-f63b65e3d542', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-09-02 07:39:38.618244', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-09-02 07:39:38.537816', `HashedPassword` = 'AQAAAAIAAYagAAAAECDk60CS+cyYwPNZ1YLGVk2l59BddGXBa7XOt7E0IfhHcqlj4xukN/3lZdEuIXQjMA=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE UNIQUE INDEX `IX_JobSkill_JobId_SkillId` ON `JobSkill` (`JobId`, `SkillId`);

CREATE INDEX `IX_JobSkill_SkillId` ON `JobSkill` (`SkillId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260902073940_ModifiedModuleSeven', '10.0.12');

CREATE INDEX `IX_Notification_UserId_IsDeleted_Status_DateCreated` ON `Notification` (`UserId`, `IsDeleted`, `Status`, `DateCreated`);

DROP INDEX IX_Notification_UserId ON Notification;

DELETE FROM `ConversationParticipant`
WHERE `Id` = '06035c74-acbd-42e2-924d-f63b65e3d542';
SELECT ROW_COUNT();


ALTER TABLE `Notification` DROP COLUMN `IsRead`;

ALTER TABLE `Notification` MODIFY `Status` int NOT NULL DEFAULT 1;

ALTER TABLE `Notification` MODIFY `CreatedBy` varchar(450) NOT NULL;

ALTER TABLE `Notification` ADD `ActionUrl` varchar(500) NULL;

ALTER TABLE `Notification` ADD `ActorAvatarUrl` varchar(500) NULL;

ALTER TABLE `Notification` ADD `ActorName` varchar(200) NULL;

ALTER TABLE `Notification` ADD `ActorUserId` varchar(255) NULL;

ALTER TABLE `Notification` ADD `DateModified` datetime(6) NULL;

ALTER TABLE `Notification` ADD `DateRead` datetime(6) NULL;

ALTER TABLE `Notification` ADD `SourceEntityId` char(36) NULL;

ALTER TABLE `Notification` ADD `SourceEntityType` int NULL;

CREATE TABLE `CompanyReviews` (
    `Id` char(36) NOT NULL,
    `CompanyId` char(36) NOT NULL,
    `ReviewerId` varchar(255) NOT NULL,
    `OverallRating` int NOT NULL,
    `WorkLifeBalanceRating` int NOT NULL,
    `CompensationRating` int NOT NULL,
    `JobSecurityRating` int NOT NULL,
    `ManagementRating` int NOT NULL,
    `CultureRating` int NOT NULL,
    `Title` longtext NOT NULL,
    `Content` longtext NOT NULL,
    `JobTitle` longtext NOT NULL,
    `Location` longtext NULL,
    `IsCurrentEmployee` tinyint(1) NOT NULL,
    `YearsAtCompany` int NULL,
    `HelpfulCount` int NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CompanyReviews_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CompanyReviews_Users_ReviewerId` FOREIGN KEY (`ReviewerId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `SavedJobSearch` (
    `Id` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `Keyword` longtext NULL,
    `Location` longtext NULL,
    `JobCategoryId` char(36) NULL,
    `EmploymentType` varchar(50) NULL,
    `WorkPlaceType` varchar(50) NULL,
    `ExperienceLevel` varchar(50) NULL,
    `MinSalary` decimal(18,2) NULL,
    `EmailNotificationsEnabled` tinyint(1) NOT NULL,
    `LastNotifiedAt` datetime(6) NULL,
    `DateCreated` datetime(6) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SavedJobSearch_JobCategory_JobCategoryId` FOREIGN KEY (`JobCategoryId`) REFERENCES `JobCategory` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_SavedJobSearch_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

UPDATE `Conversation` SET `DateCreated` = '2026-09-06 06:01:11.646496'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('48af85db-eaf3-429f-8e66-f4f6b2ca8f30', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-09-06 06:01:11.647459', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-09-06 06:01:11.569870', `HashedPassword` = 'AQAAAAIAAYagAAAAEL4y+wYdQBs8L+AdUXKU5yBt30NeCU4AUyXN6gBLjzPuOJiy7fgKr2v2wiZgrcLbAw=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE INDEX `IX_Notification_ActorUserId` ON `Notification` (`ActorUserId`);

CREATE INDEX `IX_CompanyReviews_CompanyId` ON `CompanyReviews` (`CompanyId`);

CREATE INDEX `IX_CompanyReviews_ReviewerId` ON `CompanyReviews` (`ReviewerId`);

CREATE INDEX `IX_SavedJobSearch_JobCategoryId` ON `SavedJobSearch` (`JobCategoryId`);

CREATE INDEX `IX_SavedJobSearch_ProfessionalProfileId` ON `SavedJobSearch` (`ProfessionalProfileId`);

ALTER TABLE `Notification` ADD CONSTRAINT `FK_Notification_Users_ActorUserId` FOREIGN KEY (`ActorUserId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260906060113_NotificationModule', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = '48af85db-eaf3-429f-8e66-f4f6b2ca8f30';
SELECT ROW_COUNT();


ALTER TABLE `JobApplication` ADD `LastActionedByRecruiterProfileId` char(36) NULL;

CREATE TABLE `Events` (
    `Id` char(36) NOT NULL,
    `CompanyId` char(36) NOT NULL,
    `RecruiterProfileId` char(36) NOT NULL,
    `Title` longtext NOT NULL,
    `Description` longtext NOT NULL,
    `CoverImageUrl` longtext NULL,
    `CoverImagePublicId` longtext NULL,
    `EventType` int NOT NULL,
    `Category` longtext NULL,
    `StartDateTime` datetime(6) NOT NULL,
    `EndDateTime` datetime(6) NOT NULL,
    `LocationType` int NOT NULL,
    `Location` longtext NULL,
    `OnlineMeetingLink` longtext NULL,
    `Visibility` int NOT NULL,
    `RegistrationType` int NOT NULL,
    `AttendeeLimit` int NULL,
    `Status` int NOT NULL,
    `PublishedAt` datetime(6) NULL,
    `CancelledAt` datetime(6) NULL,
    `CancellationReason` longtext NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `DateModified` datetime(6) NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Events_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Events_RecruiterProfiles_RecruiterProfileId` FOREIGN KEY (`RecruiterProfileId`) REFERENCES `RecruiterProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `EventAgendaItems` (
    `Id` char(36) NOT NULL,
    `EventId` char(36) NOT NULL,
    `Title` longtext NOT NULL,
    `Description` longtext NULL,
    `StartTime` datetime(6) NOT NULL,
    `EndTime` datetime(6) NOT NULL,
    `DisplayOrder` int NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EventAgendaItems_Events_EventId` FOREIGN KEY (`EventId`) REFERENCES `Events` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `EventRegistrations` (
    `Id` char(36) NOT NULL,
    `EventId` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `FullName` longtext NOT NULL,
    `Email` longtext NOT NULL,
    `Phone` longtext NOT NULL,
    `CompanyOrganization` longtext NULL,
    `JobTitle` longtext NULL,
    `GoalsForAttending` longtext NULL,
    `ResumeUrl` longtext NULL,
    `ResumePublicId` longtext NULL,
    `Status` int NOT NULL,
    `RegisteredAt` datetime(6) NOT NULL,
    `CancelledAt` datetime(6) NULL,
    `CreatedBy` longtext NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EventRegistrations_Events_EventId` FOREIGN KEY (`EventId`) REFERENCES `Events` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EventRegistrations_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `EventSpeakers` (
    `Id` char(36) NOT NULL,
    `EventId` char(36) NOT NULL,
    `Name` longtext NOT NULL,
    `Title` longtext NULL,
    `Company` longtext NULL,
    `PhotoUrl` longtext NULL,
    `LinkedInUrl` longtext NULL,
    `DisplayOrder` int NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EventSpeakers_Events_EventId` FOREIGN KEY (`EventId`) REFERENCES `Events` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `SavedEvents` (
    `Id` char(36) NOT NULL,
    `EventId` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SavedEvents_Events_EventId` FOREIGN KEY (`EventId`) REFERENCES `Events` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_SavedEvents_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE
);

UPDATE `Conversation` SET `DateCreated` = '2026-09-08 10:47:37.735311'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('c5e5be59-f9fa-4ace-ad3e-9e0c8616061c', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-09-08 10:47:37.736321', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-09-08 10:47:37.635106', `HashedPassword` = 'AQAAAAIAAYagAAAAELD7zodPPy+OKGhzmCnWKvdtp7RseDuMNic8c6H52JuznGSxEPwLpys7+riFzSpxAQ=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE INDEX `IX_JobApplication_LastActionedByRecruiterProfileId` ON `JobApplication` (`LastActionedByRecruiterProfileId`);

CREATE INDEX `IX_EventAgendaItems_EventId` ON `EventAgendaItems` (`EventId`);

CREATE INDEX `IX_EventRegistrations_EventId` ON `EventRegistrations` (`EventId`);

CREATE INDEX `IX_EventRegistrations_ProfessionalProfileId` ON `EventRegistrations` (`ProfessionalProfileId`);

CREATE INDEX `IX_Events_CompanyId` ON `Events` (`CompanyId`);

CREATE INDEX `IX_Events_RecruiterProfileId` ON `Events` (`RecruiterProfileId`);

CREATE INDEX `IX_EventSpeakers_EventId` ON `EventSpeakers` (`EventId`);

CREATE INDEX `IX_SavedEvents_EventId` ON `SavedEvents` (`EventId`);

CREATE INDEX `IX_SavedEvents_ProfessionalProfileId` ON `SavedEvents` (`ProfessionalProfileId`);

ALTER TABLE `JobApplication` ADD CONSTRAINT `FK_JobApplication_RecruiterProfiles_LastActionedByRecruiterProf~` FOREIGN KEY (`LastActionedByRecruiterProfileId`) REFERENCES `RecruiterProfiles` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260908104739_EventModuleStarted', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = 'c5e5be59-f9fa-4ace-ad3e-9e0c8616061c';
SELECT ROW_COUNT();


CREATE TABLE `AnalyticsEvent` (
    `Id` char(36) NOT NULL,
    `EventType` int NOT NULL,
    `SubjectType` int NOT NULL,
    `SubjectId` char(36) NOT NULL,
    `ActorUserId` varchar(255) NULL,
    `ReferrerSource` int NULL,
    `DateCreated` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AnalyticsEvent_Users_ActorUserId` FOREIGN KEY (`ActorUserId`) REFERENCES `Users` (`Id`) ON DELETE SET NULL
);

UPDATE `Conversation` SET `DateCreated` = '2026-09-12 22:47:56.455101'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('11ef98cb-df77-423d-806c-89833b448aca', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-09-12 22:47:56.456082', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-09-12 22:47:56.375540', `HashedPassword` = 'AQAAAAIAAYagAAAAENJd4BwyHr4b4MBK4J/dfA+f2h7lOqN85UFfKPvfRikFM3ijY4NxmFZXUvv5Vd2MQg=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE INDEX `IX_AnalyticsEvent_ActorUserId_DateCreated` ON `AnalyticsEvent` (`ActorUserId`, `DateCreated`);

CREATE INDEX `IX_AnalyticsEvent_SubjectType_SubjectId_DateCreated` ON `AnalyticsEvent` (`SubjectType`, `SubjectId`, `DateCreated`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260912224757_DueToAnalyticsModule', '10.0.12');

ALTER TABLE `Notification` DROP CONSTRAINT `FK_Notification_Users_ActorUserId`;

DELETE FROM `ConversationParticipant`
WHERE `Id` = '11ef98cb-df77-423d-806c-89833b448aca';
SELECT ROW_COUNT();


UPDATE `Conversation` SET `DateCreated` = '2026-09-13 11:58:54.575295'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('695078e2-cf76-47ee-b37f-095659a41dbe', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-09-13 11:58:54.576301', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-09-13 11:58:54.489711', `HashedPassword` = 'AQAAAAIAAYagAAAAEJVlEBdRzNH/xbobmSxIaqNLUFysHlcoxrjmERaOxrwgFMmZvBqZFsY7UlazkLn1Gw=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


ALTER TABLE `Notification` ADD CONSTRAINT `FK_Notification_Users_ActorUserId` FOREIGN KEY (`ActorUserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260913115856_Cascade', '10.0.12');

ALTER TABLE `Notification` DROP CONSTRAINT `FK_Notification_Users_ActorUserId`;

DELETE FROM `ConversationParticipant`
WHERE `Id` = '695078e2-cf76-47ee-b37f-095659a41dbe';
SELECT ROW_COUNT();


ALTER TABLE `Companies` ADD `CoverImagePublicId` longtext NULL;

ALTER TABLE `Companies` ADD `CoverImageUrl` longtext NULL;

UPDATE `Conversation` SET `DateCreated` = '2026-09-19 09:06:45.157812'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('bc766de4-bea8-457d-aedd-f25488533e49', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-09-19 09:06:45.158760', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-09-19 09:06:45.077725', `HashedPassword` = 'AQAAAAIAAYagAAAAECnhGgrvv4LsoGmxxcN/AotzwhVHYpk5jfMZ0mf+zCgepBUqbO0vZmeTc94z1oHzQw=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


ALTER TABLE `Notification` ADD CONSTRAINT `FK_Notification_Users_ActorUserId` FOREIGN KEY (`ActorUserId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260919090646_Initial', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = 'bc766de4-bea8-457d-aedd-f25488533e49';
SELECT ROW_COUNT();


CREATE TABLE `Departments` (
    `Id` char(36) NOT NULL,
    `CompanyId` char(36) NOT NULL,
    `Name` longtext NOT NULL,
    `Description` longtext NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `DateModified` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Departments_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE
);

UPDATE `Conversation` SET `DateCreated` = '2026-09-20 18:43:09.125860'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('19b794c6-6815-4862-b96a-c77f3a77186c', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-09-20 18:43:09.126830', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-09-20 18:43:09.044814', `HashedPassword` = 'AQAAAAIAAYagAAAAECbyXdueiLkOUjcqQTYQrV68VeTkrduM/l/Qw0s4Y9xWKWjgTfNu37c7xksX5FD7GA=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE INDEX `IX_Departments_CompanyId` ON `Departments` (`CompanyId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260920184310_Update', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = '19b794c6-6815-4862-b96a-c77f3a77186c';
SELECT ROW_COUNT();


CREATE TABLE `SavedCandidates` (
    `Id` char(36) NOT NULL,
    `RecruiterProfileId` char(36) NOT NULL,
    `ProfessionalProfileId` char(36) NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `CreatedBy` longtext NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SavedCandidates_ProfessionalProfiles_ProfessionalProfileId` FOREIGN KEY (`ProfessionalProfileId`) REFERENCES `ProfessionalProfiles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_SavedCandidates_RecruiterProfiles_RecruiterProfileId` FOREIGN KEY (`RecruiterProfileId`) REFERENCES `RecruiterProfiles` (`Id`) ON DELETE CASCADE
);

UPDATE `Conversation` SET `DateCreated` = '2026-09-21 07:41:07.172405'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('8bbade0c-4e9a-4303-9a4f-5df751b11e33', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-09-21 07:41:07.173518', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-09-21 07:41:07.088151', `HashedPassword` = 'AQAAAAIAAYagAAAAEB4ce3yxBvsMw02x2RHyEpf1leXXeHjqnyq2jp/LhZ2EHj6M+mxgnVp1gKOZKGUXgQ=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE INDEX `IX_SavedCandidates_ProfessionalProfileId` ON `SavedCandidates` (`ProfessionalProfileId`);

CREATE INDEX `IX_SavedCandidates_RecruiterProfileId` ON `SavedCandidates` (`RecruiterProfileId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260921074108_AddSavedCandidate', '10.0.12');

ALTER TABLE `Departments` DROP CONSTRAINT `FK_Departments_Companies_CompanyId`;

ALTER TABLE `Job` DROP CONSTRAINT `FK_Job_JobCategory_JobCategoryId`;

ALTER TABLE `SavedJobSearch` DROP CONSTRAINT `FK_SavedJobSearch_JobCategory_JobCategoryId`;

DROP TABLE `JobCategory`;

DELETE FROM `ConversationParticipant`
WHERE `Id` = '8bbade0c-4e9a-4303-9a4f-5df751b11e33';
SELECT ROW_COUNT();


ALTER TABLE Departments RENAME Deparment;

ALTER TABLE `SavedJobSearch` CHANGE `JobCategoryId` `DepartmentId1` char(36) NULL;

ALTER TABLE `SavedJobSearch` RENAME INDEX `IX_SavedJobSearch_JobCategoryId` TO `IX_SavedJobSearch_DepartmentId1`;

ALTER TABLE `Deparment` RENAME INDEX `IX_Departments_CompanyId` TO `IX_Deparment_CompanyId`;

ALTER TABLE `SavedJobSearch` ADD `DepartmentId` char(36) NULL;

ALTER TABLE `Deparment` MODIFY `Name` varchar(100) NOT NULL;

UPDATE `Conversation` SET `DateCreated` = '2026-09-24 04:29:58.864550'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('e199a51e-def7-49d4-9663-08c2e0616969', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-09-24 04:29:58.865494', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-09-24 04:29:58.783620', `HashedPassword` = 'AQAAAAIAAYagAAAAEFQwdoK83o9eLb8A3e5UgZTrxXzV4uRqy0uXH98fyubyEhyZPyUMS7WMGRO8imKYbg=='
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


CREATE INDEX `IX_SavedJobSearch_DepartmentId` ON `SavedJobSearch` (`DepartmentId`);

CREATE UNIQUE INDEX `IX_Deparment_Name` ON `Deparment` (`Name`);

ALTER TABLE `Deparment` ADD CONSTRAINT `FK_Deparment_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Companies` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Job` ADD CONSTRAINT `FK_Job_Deparment_JobCategoryId` FOREIGN KEY (`JobCategoryId`) REFERENCES `Deparment` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `SavedJobSearch` ADD CONSTRAINT `FK_SavedJobSearch_Deparment_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `Deparment` (`Id`) ON DELETE SET NULL;

ALTER TABLE `SavedJobSearch` ADD CONSTRAINT `FK_SavedJobSearch_Deparment_DepartmentId1` FOREIGN KEY (`DepartmentId1`) REFERENCES `Deparment` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260924043000_CommotJobCategory', '10.0.12');

DELETE FROM `ConversationParticipant`
WHERE `Id` = 'e199a51e-def7-49d4-9663-08c2e0616969';
SELECT ROW_COUNT();


ALTER TABLE `Users` DROP COLUMN `IsActive`;

ALTER TABLE `Users` ADD `Status` int NOT NULL DEFAULT 0;

ALTER TABLE `Report` MODIFY `PostId` char(36) NULL;

ALTER TABLE `Report` ADD `ContentType` longtext NOT NULL;

ALTER TABLE `Companies` ADD `Status` int NOT NULL DEFAULT 0;

UPDATE `Conversation` SET `DateCreated` = '2026-10-01 07:11:30.798810'
WHERE `Id` = 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2';
SELECT ROW_COUNT();


INSERT INTO `ConversationParticipant` (`Id`, `ConversationId`, `CreatedBy`, `IsDeleted`, `IsHidden`, `IsMuted`, `IsPinned`, `JoinedAt`, `LastReadAt`, `UserId`)
VALUES ('468d8cc8-2303-4128-aff8-689e01e5478a', 'b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2', 'c117635d-96e0-409b-9fae-72976ec9c42a', FALSE, FALSE, FALSE, FALSE, '2026-10-01 07:11:30.800147', NULL, 'c117635d-96e0-409b-9fae-72976ec9c42a');
SELECT ROW_COUNT();


UPDATE `Users` SET `DateCreated` = '2026-10-01 07:11:30.693463', `HashedPassword` = 'AQAAAAIAAYagAAAAEL9NXwCKuScg5iF1EazblSt0vKcD/QYV+2opRua2lfl13OUYcJzCKMy+z7GKDalZkQ==', `Status` = 1
WHERE `Id` = 'c117635d-96e0-409b-9fae-72976ec9c42a';
SELECT ROW_COUNT();


INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261001071133_AdminModule', '10.0.12');

COMMIT;

