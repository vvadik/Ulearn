BEGIN TRANSACTION


-- AspNetRoles

ALTER TABLE AspNetRoles
ADD [ConcurrencyStamp] [nvarchar](max) NULL

ALTER TABLE AspNetRoles 
ADD [NormalizedName] [nvarchar](256) NULL

-- AspNetUsers

ALTER TABLE AspNetUsers
ADD [ConcurrencyStamp] [nvarchar](max) NULL

ALTER TABLE AspNetUsers
ADD [LockoutEnd] [datetimeoffset](7) NULL

ALTER TABLE AspNetUsers
ADD [NormalizedEmail] [nvarchar](256) NULL

ALTER TABLE AspNetUsers
ADD [NormalizedUserName] [nvarchar](256) NULL



COMMIT TRANSACTION