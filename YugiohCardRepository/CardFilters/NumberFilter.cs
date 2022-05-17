using System;
namespace YugiohCardRepository.CardFilters
{
	public class NumberFilter
	{
        public NumberOperator NumberOperator { get; init; }
		public int RightOperand { get; init; }

        public NumberFilter(NumberOperator numberOperator, int rightOperand)
        {
            NumberOperator = numberOperator;
            RightOperand = rightOperand;
        }
    }
}