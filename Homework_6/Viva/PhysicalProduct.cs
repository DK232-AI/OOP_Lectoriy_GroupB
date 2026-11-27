using System;

namespace Marketplace.Domain;

public class PhysicalProduct : Product
{
    public double WeightKg { get; }
    public string? WarehouseCode { get; set; }

    public PhysicalProduct(double weightKg, string? warehouseCode) : base()
    {
        EnsureWeight(weightKg);
        WeightKg = weightKg;
        WarehouseCode = warehouseCode;
    }

    public string DeliveryInfo => WarehouseCode ?? "склад не назначен";

    public string GetFullInfo() => $"{Title}: {base.GetDescription()}, вес: {WeightKg} кг, {DeliveryInfo}";

    private static void EnsureWeight(double weightKg)
    {
        if (!double.IsFinite(weightKg) || weightKg <= 0)
            throw new ArgumentOutOfRangeException(nameof(weightKg), "Вес должен быть положительным конечным числом.");
    }
}
