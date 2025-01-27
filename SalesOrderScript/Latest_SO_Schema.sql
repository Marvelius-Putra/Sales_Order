
use [SQL2016.OL_DEV]
/*
Run this script on:

        .\SQL2016.OL_DEV    -  This database will be modified

to synchronize it with:

        .\SQL2016.so

You are recommended to back up your database before running this script

*/
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[COM_CUSTOMER]'
GO
IF OBJECT_ID(N'[dbo].[COM_CUSTOMER]', 'U') IS NULL
CREATE TABLE [dbo].[COM_CUSTOMER]
(
[COM_CUSTOMER_ID] [int] NOT NULL IDENTITY(1, 1),
[CUSTOMER_NAME] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_COM_CUSTOMER] on [dbo].[COM_CUSTOMER]'
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PK_COM_CUSTOMER]', 'PK') AND parent_object_id = OBJECT_ID(N'[dbo].[COM_CUSTOMER]', 'U'))
ALTER TABLE [dbo].[COM_CUSTOMER] ADD CONSTRAINT [PK_COM_CUSTOMER] PRIMARY KEY CLUSTERED  ([COM_CUSTOMER_ID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[SO_ITEM]'
GO
IF OBJECT_ID(N'[dbo].[SO_ITEM]', 'U') IS NULL
CREATE TABLE [dbo].[SO_ITEM]
(
[SO_ITEM_ID] [bigint] NOT NULL IDENTITY(1, 1),
[SO_ORDER_ID] [bigint] NOT NULL CONSTRAINT [DF_Table_1_SALES_SO_ID] DEFAULT ((-99)),
[ITEM_NAME] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_SO_ITEM_ITEM_NAME] DEFAULT (''),
[QUANTITY] [int] NOT NULL CONSTRAINT [DF_SO_ITEM_QUANTITY] DEFAULT ((-99)),
[PRICE] [float] NOT NULL CONSTRAINT [DF_SO_ITEM_PRICE] DEFAULT ((0.0))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_SO_ITEM] on [dbo].[SO_ITEM]'
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PK_SO_ITEM]', 'PK') AND parent_object_id = OBJECT_ID(N'[dbo].[SO_ITEM]', 'U'))
ALTER TABLE [dbo].[SO_ITEM] ADD CONSTRAINT [PK_SO_ITEM] PRIMARY KEY CLUSTERED  ([SO_ITEM_ID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[SO_ORDER]'
GO
IF OBJECT_ID(N'[dbo].[SO_ORDER]', 'U') IS NULL
CREATE TABLE [dbo].[SO_ORDER]
(
[SO_ORDER_ID] [bigint] NOT NULL IDENTITY(1, 1),
[ORDER_NO] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_SO_ORDER_ORDER_NO] DEFAULT (''),
[ORDER_DATE] [datetime] NOT NULL CONSTRAINT [DF_SO_ORDER_ORDER_DATE] DEFAULT ('1900-01-01'),
[COM_CUSTOMER_ID] [int] NOT NULL CONSTRAINT [DF_SO_ORDER_COM_CUSTOMER_ID] DEFAULT ('-99'),
[ADDRESS] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_SO_ORDER_ADDRESS] DEFAULT ('')
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_SO_ORDER] on [dbo].[SO_ORDER]'
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PK_SO_ORDER]', 'PK') AND parent_object_id = OBJECT_ID(N'[dbo].[SO_ORDER]', 'U'))
ALTER TABLE [dbo].[SO_ORDER] ADD CONSTRAINT [PK_SO_ORDER] PRIMARY KEY CLUSTERED  ([SO_ORDER_ID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
-- This statement writes to the SQL Server Log so SQL Monitor can show this deployment.
IF HAS_PERMS_BY_NAME(N'sys.xp_logevent', N'OBJECT', N'EXECUTE') = 1
BEGIN
    DECLARE @databaseName AS nvarchar(2048), @eventMessage AS nvarchar(2048)
    SET @databaseName = REPLACE(REPLACE(DB_NAME(), N'\', N'\\'), N'"', N'\"')
    SET @eventMessage = N'Redgate SQL Compare: { "deployment": { "description": "Redgate SQL Compare deployed to ' + @databaseName + N'", "database": "' + @databaseName + N'" }}'
    EXECUTE sys.xp_logevent 55000, @eventMessage
END
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	PRINT 'The database update failed'
END
GO



-- MARVELIUS - ADDED DATABASE

-- Insert data into COM_CUSTOMER
SET IDENTITY_INSERT COM_CUSTOMER ON;
INSERT INTO COM_CUSTOMER (COM_CUSTOMER_ID, CUSTOMER_NAME) VALUES
    (1, 'PROFES'),
    (2, 'TITAN'),
    (3, 'DIPS');
SET IDENTITY_INSERT COM_CUSTOMER OFF;
GO

-- Insert data into SO_ITEM
SET IDENTITY_INSERT SO_ITEM ON;
INSERT INTO SO_ITEM (SO_ITEM_ID, SO_ORDER_ID, ITEM_NAME, QUANTITY, PRICE) VALUES
    (1, 101, 'KULKAS', 2, 5000000),
    (2, 101, 'AC', 3, 9000000),
    (3, 101, 'DESKTOP 6', 6, 60000000),
    (4, 101, 'TV', 1, 7900000),
    (5, 102, 'LAPTOP', 1, 15000000),
    (6, 102, 'SMARTPHONE', 2, 12000000),
    (7, 103, 'TABLET', 3, 8000000),
    (8, 103, 'MONITOR', 2, 4000000),
    (9, 104, 'HEADPHONES', 5, 500000),
    (10, 104, 'PRINTER', 1, 2500000);
SET IDENTITY_INSERT SO_ITEM OFF;
GO

-- Insert data into SO_ORDER
SET IDENTITY_INSERT SO_ORDER ON;
INSERT INTO SO_ORDER (SO_ORDER_ID, ORDER_NO, ORDER_DATE, COM_CUSTOMER_ID, ADDRESS) VALUES
    (101, '50_001', '2011-02-24', 1, 'Indonesia'),
    (102, '50_002', '2023-12-15', 2, 'Jakarta'),
    (103, '50_003', '2024-01-10', 3, 'Bandung'),
    (104, '50_004', '2024-01-25', 1, 'Surabaya');
SET IDENTITY_INSERT SO_ORDER OFF;
GO

-- Stored Procedure to Get All Customer Sales Orders
CREATE PROCEDURE GET_CUSTOMER_SALES_ORDERS
AS
BEGIN
    SELECT 
        o.SO_ORDER_ID AS Id,
        o.ORDER_NO AS OrderNumber,
        o.ORDER_DATE AS OrderDate,
        c.CUSTOMER_NAME AS Customer
    FROM 
        SO_ORDER o
    INNER JOIN 
        COM_CUSTOMER c ON o.COM_CUSTOMER_ID = c.COM_CUSTOMER_ID;
END;
GO

-- Stored Procedure to Get Customer Details
CREATE PROCEDURE GET_CUSTOMERS
AS
BEGIN
    SELECT 
        COM_CUSTOMER_ID AS Id,
        CUSTOMER_NAME AS Name
    FROM 
        COM_CUSTOMER
    ORDER BY CUSTOMER_NAME
END;
GO

-- Stored Procedure to Add Sales Order
CREATE PROCEDURE INSERT_SALES_ORDER
    @OrderNo NVARCHAR(50),
    @OrderDate DATETIME,
    @CustomerId INT,
    @Address NVARCHAR(200)
AS
BEGIN
    INSERT INTO SO_ORDER (ORDER_NO, ORDER_DATE, COM_CUSTOMER_ID, ADDRESS)
    VALUES (@OrderNo, @OrderDate, @CustomerId, @Address);

    SELECT SCOPE_IDENTITY() AS NewOrderId; 
END;
GO

-- Stored Procedure to Add Sales Order Items
CREATE PROCEDURE INSERT_SALES_ORDER_ITEM
    @OrderId INT,
    @ItemName NVARCHAR(100),
    @Quantity INT,
    @Price DECIMAL(18, 2)
AS
BEGIN
    -- Check if the items is not exist
    IF NOT EXISTS (SELECT 1 FROM SO_ITEM WHERE SO_ORDER_ID = @OrderId AND ITEM_NAME = @ItemName)
    BEGIN
        INSERT INTO SO_ITEM (SO_ORDER_ID, ITEM_NAME, QUANTITY, PRICE)
        VALUES (@OrderId, @ItemName, @Quantity, @Price);
    END
    ELSE
    BEGIN
        -- IGNORE The existed Item
        PRINT 'Item is already exist';
    END
END;
GO

-- Stored Procedure to Get Sales Order and their Item based on ID
CREATE PROCEDURE GET_SALES_ORDER_BY_ID
    @Id INT
AS
BEGIN
    -- Get Customer Sales Order Data
    SELECT 
        so.SO_ORDER_ID AS Id,
        so.ORDER_NO AS OrderNumber,
        so.ORDER_DATE AS OrderDate,
        so.COM_CUSTOMER_ID AS CustomerId, -- Tambahkan CustomerId
        c.CUSTOMER_NAME AS Customer,
        so.ADDRESS AS Address
    FROM SO_ORDER so
    JOIN com_Customer c ON so.COM_CUSTOMER_ID = c.COM_CUSTOMER_ID
    WHERE so.SO_ORDER_ID = @Id;

    -- Get Sales Order Item
    SELECT 
        soi.SO_ITEM_ID AS Id,
        soi.ITEM_NAME AS ItemName,
        soi.QUANTITY AS Quantity,
        soi.PRICE AS Price
    FROM SO_ITEM soi
    WHERE soi.SO_ORDER_ID = @Id;
END;
GO

-- Stored Procedure to Update Sales Order and their Item
CREATE PROCEDURE UPDATE_SALES_ORDER
	@Order_Id INT,
    @OrderNo VARCHAR(50),
    @OrderDate DATE,
    @CustomerId INT,
    @Address VARCHAR(255)
AS
BEGIN
	UPDATE SO_ORDER
	SET ORDER_NO = @OrderNo,
		ORDER_DATE = @OrderDate,
		COM_CUSTOMER_ID = @CustomerId,		
		Address = @Address
	WHERE SO_ORDER_ID = @Order_Id;
END;
GO

-- Stored Procedure to Update and Insert Sales Order Items
CREATE PROCEDURE UPSERT_SALES_ORDER_ITEM
    @ItemId INT = NULL, -- Bisa NULL untuk item baru
    @OrderId INT,
    @ItemName VARCHAR(255),
    @Quantity INT,
    @Price DECIMAL(18, 2)
AS
BEGIN
	-- Check if it is a new Item
    IF @ItemId = 0
    BEGIN
        INSERT INTO SO_ITEM (SO_ORDER_ID, ITEM_NAME, Quantity, Price)
        VALUES (@OrderId, @ItemName, @Quantity, @Price);
    END
    ELSE
	-- Check if is is a modified item
    BEGIN
        UPDATE SO_ITEM
        SET ITEM_NAME = @ItemName,
            Quantity = @Quantity,
            Price = @Price
        WHERE SO_ITEM_ID = @ItemId AND SO_ORDER_ID = @OrderId;
    END
END;
GO

-- Stored Procedure to delete items
CREATE PROCEDURE DELETE_ORDER_BY_ID
    @SO_ORDER_ID INT
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
        DELETE FROM SO_ITEM
        WHERE SO_ORDER_ID = @SO_ORDER_ID;

        DELETE FROM SO_ORDER
        WHERE SO_ORDER_ID = @SO_ORDER_ID;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Terjadi kesalahan: ' + ERROR_MESSAGE();
    END CATCH
END;
GO

-- Stored Procedure only to delete Sales Order Item
CREATE PROCEDURE DELETE_ORDER_ITEM_BY_ID
    @SO_ITEM_ID VARCHAR(MAX)
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
        DELETE FROM SO_ITEM
        WHERE SO_ITEM_ID IN (SELECT value FROM STRING_SPLIT(@SO_ITEM_ID, ','));

        COMMIT TRANSACTION;

        PRINT 'Data berhasil dihapus.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Terjadi kesalahan: ' + ERROR_MESSAGE();
    END CATCH
END;
GO
