using System;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The data key
    /// </summary>
    public class DataKey<TValueType>
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataKey{TValueType}" /> class.
        /// </summary>
        public DataKey(Type locatedType, string fieldName)
        {
            if (locatedType == null)
            {
                throw new ArgumentNullException(nameof(locatedType));
            }

            if (fieldName == null)
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(fieldName));
            }

            Id = $"{locatedType.FullName}:{fieldName}";
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        public string Id { get; }
        #endregion

        #region Public Indexers
        /// <summary>
        ///     Gets the <see cref="TValueType" /> with the specified context.
        /// </summary>
        public TValueType this[DataContext context]
        {
            get { return context.Get(this); }
            set { context.Add(this, value); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return $"name: {ShortNameHelper.GetShortName(Id)} - Type: {typeof(TValueType).FullName}";
        }
        #endregion
    }
}