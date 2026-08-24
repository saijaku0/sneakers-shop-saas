SELECT oi.WarehouseItemId, oi.Quantity, oi.UnitPrice, oi.DiscountAmount
FROM OrderItems oi
JOIN Orders o ON o.Id = oi.OrderId
WHERE o.UserId = '08C79711-B5AD-4E9E-91B0-08DEFE9D6C4D';