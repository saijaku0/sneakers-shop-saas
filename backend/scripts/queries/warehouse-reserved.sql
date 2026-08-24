SELECT Id, Quantity, ReservedQuantity, (Quantity - ReservedQuantity) AS Available
FROM WarehouseItems WHERE Id IN ('271D9243-EB49-4E43-864E-0057335BC1C4');