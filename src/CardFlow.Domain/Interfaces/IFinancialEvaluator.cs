namespace CardFlow.Domain.Interfaces;

// Giao diện chung cho các đối tượng có phát sinh chi phí và doanh thu.
// Tách riêng interface này để module kế toán chỉ làm việc với các chỉ số tiền tệ.
public interface IFinancialEvaluator
{
    decimal UnitBlankCost { get; }
    decimal CalculateUnitProcessingCost();
    decimal UnitSellingPrice { get; }

    decimal CalculateUnitTotalCost() => UnitBlankCost + CalculateUnitProcessingCost();
    decimal CalculateUnitProfit() => UnitSellingPrice - CalculateUnitTotalCost();
    decimal CalculateUnitProfitMargin() => UnitSellingPrice > 0 
        ? Math.Round((CalculateUnitProfit() / UnitSellingPrice) * 100, 2) 
        : 0;
}
