using CLSPhase2.Dal.Entities.CSGW;

namespace CLSPhase2.Dal.Entities.CLS
{
    public class TtbCalculateWorstKyc
    {
        public class CalculateWorstKycRequest<T, U, V>
        {
            public CalculateWorstKycBody<T, U, V> data;
            public CalRiskRequestHeader header { get; set; }
            public string Uri { get; set; }
        }
        public class CalculateWorstKycBody<T, U, V>
        {
            public T body { get; set; }
            public IEnumerable<CalculateWorstKycIndividual<U>> relatedCustIndividual { get; set; }
            public IEnumerable<CalculateWorstKycCustCorporate<V>> relatedCustCorporate { get; set; }
            public long EntityId { get; set; }
        }

        public class CalculateWorstKycIndividual<U>
        {
            public U body { get; set; }

            public long EntityId { get; set; }
        }
        public class CalculateWorstKycCustCorporate<V>
        {
            public V body { get; set; }
            public long EntityId { get; set; }
        }

        public class EntityRiskLevel
        {
            public long EntityId { get; set; }
            public bool IsBorrower { get; set; }
            public int maxRisk { get; set; }
            public string maxRiskRM { get; set; }
            public int kycLevel
            {
                get
                {
                    if (maxRiskRM is null)
                        return 0;
                    return (int)(EnumKYCRiskLevel)Enum.Parse(typeof(EnumKYCRiskLevel), maxRiskRM);
                }
            }
            public bool IsSuccess { get; set; }

        }
    }
}
