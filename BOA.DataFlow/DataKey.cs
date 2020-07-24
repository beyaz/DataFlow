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
        public DataKey(string name)
        {
            Name = name;
            Id   = IdGenerator.GetNextId().ToString();
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name { get; }
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
            if (Name == null)
            {
                return $"Id: {Id} - Type: {typeof(TValueType).FullName}";
            }

            return $"name: {Name} - Type: {typeof(TValueType).FullName}";
        }
        #endregion
    }
}