-- Cleanup
DELETE FROM WarehouseItems;
DELETE FROM ProductVariants;
DELETE FROM Products;
DELETE FROM Categories;
DELETE FROM Brands;

DECLARE @now datetimeoffset = SYSDATETIMEOFFSET();

-- Reference data
DECLARE @Brands TABLE (Id uniqueidentifier, Name nvarchar(100), Idx int);
INSERT INTO @Brands VALUES
(NEWID(),'Nike',0),(NEWID(),'Adidas',1),(NEWID(),'Puma',2),
(NEWID(),'New Balance',3),(NEWID(),'Asics',4),(NEWID(),'Reebok',5);

DECLARE @Categories TABLE (Id uniqueidentifier, Name nvarchar(100), Idx int);
INSERT INTO @Categories VALUES
(NEWID(),'Running',0),(NEWID(),'Sneakers',1),(NEWID(),'Basketball',2),
(NEWID(),'Lifestyle',3),(NEWID(),'Training',4);

INSERT INTO Brands (Id, Name, CreatedAt) SELECT Id, Name, @now FROM @Brands;
INSERT INTO Categories (Id, Name, CreatedAt) SELECT Id, Name, @now FROM @Categories;

-- Color palette: color + background + foreground
DECLARE @Palette TABLE (Idx int, Name nvarchar(20), Bg char(6), Fg char(6));
INSERT INTO @Palette VALUES
(0,'Black','1a1a1a','ffffff'),(1,'White','f5f5f5','111111'),
(2,'Red','c0392b','ffffff'),(3,'Blue','2c3e50','ffffff'),
(4,'Green','27ae60','ffffff'),(5,'Grey','7f8c8d','ffffff'),
(6,'Volt','d3ff00','111111'),(7,'Orange','e67e22','ffffff');

DECLARE @Genders TABLE (Idx int, Val nvarchar(20));
INSERT INTO @Genders VALUES (0,'Men'),(1,'Women'),(2,'Unisex'),(3,'Kids');

DECLARE @Sizes TABLE (Idx int, Val decimal(4,1));
INSERT INTO @Sizes VALUES (0,23.0),(1,24.0),(2,25.0),(3,26.0),(4,27.0),(5,28.0),(6,29.0);

-- Generate 60 products
DECLARE @i int = 0;
WHILE @i < 420
BEGIN
    DECLARE @brandId uniqueidentifier = (SELECT Id FROM @Brands WHERE Idx = @i % 6);
    DECLARE @brandName nvarchar(100) = (SELECT Name FROM @Brands WHERE Idx = @i % 6);
    DECLARE @catId uniqueidentifier = (SELECT Id FROM @Categories WHERE Idx = @i % 5);
    DECLARE @gender nvarchar(20) = (SELECT Val FROM @Genders WHERE Idx = @i % 4);
    DECLARE @price decimal(10,2) = 80.0 + (@i % 12) * 10.0;

    DECLARE @pid uniqueidentifier = NEWID();
    INSERT INTO Products (Id, BrandId, CategoryId, Model, Description, BasePrice, IsActive, Gender, CreatedAt)
    VALUES (@pid, @brandId, @catId, CONCAT('Model ', @i + 1),
            CONCAT('Description for model ', @i + 1), @price, 1, @gender, @now);

    -- Number of colors: 1..8
    DECLARE @colorCount int = 1 + (@i % 8);
    DECLARE @start int = @i % 8;
    DECLARE @c int = 0;

    WHILE @c < @colorCount
    BEGIN
        DECLARE @pIdx int = (@start + @c) % 8;
        DECLARE @color nvarchar(20) = (SELECT Name FROM @Palette WHERE Idx = @pIdx);
        DECLARE @bg char(6) = (SELECT Bg FROM @Palette WHERE Idx = @pIdx);
        DECLARE @fg char(6) = (SELECT Fg FROM @Palette WHERE Idx = @pIdx);

        -- 1..3 images
        DECLARE @imgCount int = 1 + (@c % 3);
        DECLARE @baseUrl nvarchar(200) =
            CONCAT('https://placehold.co/600x600/', @bg, '/', @fg, '?text=',
                   REPLACE(@brandName,' ','+'), '+', @color);

        DECLARE @json nvarchar(max) = N'[';
        DECLARE @img int = 1;
        WHILE @img <= @imgCount
        BEGIN
            SET @json = @json + CASE WHEN @img > 1 THEN ',' ELSE '' END
                + N'{"Url":"' + @baseUrl + '+' + CAST(@img AS nvarchar(2)) + '"}';
            SET @img = @img + 1;
        END
        SET @json = @json + N']';

        DECLARE @preview nvarchar(200) = @baseUrl + '+1';

        DECLARE @vid uniqueidentifier = NEWID();
        INSERT INTO ProductVariants (Id, ProductId, Color, Images, PreviewImageUrl, CreatedAt)
        VALUES (@vid, @pid, @color, @json, @preview, @now);

        -- 2..4 sizes
        DECLARE @sizeCount int = 2 + (@c % 3);
        DECLARE @sStart int = @c % (7 - @sizeCount);
        DECLARE @s int = 0;
        WHILE @s < @sizeCount
        BEGIN
            DECLARE @sizeVal decimal(4,1) = (SELECT Val FROM @Sizes WHERE Idx = @sStart + @s);
            DECLARE @qty int = 1 + ABS(CHECKSUM(NEWID())) % 10;
            INSERT INTO WarehouseItems (Id, ProductVariantId, Quantity, ReservedQuantity, Size, CreatedAt)
            VALUES (NEWID(), @vid, @qty, 0, @sizeVal, @now);
            SET @s = @s + 1;
        END

        SET @c = @c + 1;
    END

    SET @i = @i + 1;
END

-- Verification
SELECT
  (SELECT COUNT(*) FROM Products) AS Products,
  (SELECT COUNT(*) FROM ProductVariants) AS Variants,
  (SELECT COUNT(*) FROM WarehouseItems) AS Warehouse;