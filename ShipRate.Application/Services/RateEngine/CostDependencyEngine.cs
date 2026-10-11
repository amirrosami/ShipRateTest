using ShipRate.Application.Interfaces.RateEngine;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Application.Services.RateEngine
{
    public class CostDependencyEngine : ICostDependencyEngine
    {
        public Task<List<ChargeType>> ApplyDependenciesAsync(Guid shipmentId, List<ChargeType> baseCosts, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //private readonly ICostTypeRepository _costTypeRepo;
        //private readonly ICostRuleRepository _costRuleRepo;

        //public async Task<List<Cost>> ApplyDependenciesAsync(
        //    Guid shipmentId,
        //    List<Cost> baseCosts,
        //    CancellationToken cancellationToken)
        //{
        //    var additionalCosts = new List<Cost>();

        //    // ۱. وابستگی‌های مستقیم از CostType
        //    foreach (var cost in baseCosts)
        //    {
        //        var costType = await _costTypeRepo.GetByIdAsync(cost.CostTypeId);
        //        if (costType.DependsOnCostTypeId.HasValue)
        //        {
        //            // بررسی وجود هزینه‌ی پایه
        //            var dependsCost = baseCosts.FirstOrDefault(c => c.CostTypeId == costType.DependsOnCostTypeId);
        //            if (dependsCost != null)
        //            {
        //                // ایجاد هزینه‌ی وابسته
        //                var dependentCost = Cost.Create(
        //                    shipmentId,
        //                    costType.Id,
        //                    cost.Direction,
        //                    CalculateDependentAmount(costType, dependsCost.Amount));
        //                additionalCosts.Add(dependentCost);
        //            }
        //        }
        //    }

        //    // ۲. اعمال قواعد پیچیده‌تر از CostRule
        //    var activeRules = await _costRuleRepo.GetActiveRulesAsync();
        //    foreach (var rule in activeRules)
        //    {
        //        if (await EvaluateRuleConditionAsync(rule, baseCosts, additionalCosts))
        //        {
        //            var targetCost = await CreateCostFromRuleAsync(rule, shipmentId, baseCosts, additionalCosts);
        //            additionalCosts.Add(targetCost);
        //        }
        //    }

        //    return baseCosts.Concat(additionalCosts).ToList();
        //}

        //private Money CalculateDependentAmount(CostType dependentType, Money baseAmount)
        //{
        //    if (dependentType.DependencyType == CostDependencyType.Multiplier &&
        //        dependentType.MultiplierRate.HasValue)
        //    {
        //        return baseAmount.Multiply(dependentType.MultiplierRate.Value);
        //    }
        //    // برای سایر انواع وابستگی‌ها منطق مناسب
        //    return baseAmount;
        //}

        //// سایر متدهای کمکی...



    }
}
