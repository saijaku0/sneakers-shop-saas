SELECT c.UserId, ci.WarehouseItemId, ci.Quantity
FROM Carts c
JOIN CartItems ci ON ci.CartId = c.Id;