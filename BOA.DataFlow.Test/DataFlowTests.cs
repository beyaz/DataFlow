using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The data flow tests
    /// </summary>
    [TestClass]
    public class DataFlowTests
    {
        #region Static Fields
        /// <summary>
        ///     The data bracket 0 0
        /// </summary>
        static readonly DataKey<string> data_bracket_0_0 = new DataKey<string>(nameof(data_bracket_0_0));

        /// <summary>
        ///     The data bracket 1 0
        /// </summary>
        static readonly DataKey<string> data_bracket_1_0 = new DataKey<string>(nameof(data_bracket_1_0));

        /// <summary>
        ///     The data bracket 1 1
        /// </summary>
        static readonly DataKey<string> data_bracket_1_1 = new DataKey<string>(nameof(data_bracket_1_1));

        /// <summary>
        ///     The data bracket 2 0
        /// </summary>
        static readonly DataKey<string> data_bracket_2_0 = new DataKey<string>(nameof(data_bracket_2_0));

        /// <summary>
        ///     The data bracket 2 1
        /// </summary>
        static readonly DataKey<string> data_bracket_2_1 = new DataKey<string>(nameof(data_bracket_2_1));

        /// <summary>
        ///     The data bracket 2 2
        /// </summary>
        static readonly DataKey<string> data_bracket_2_2 = new DataKey<string>(nameof(data_bracket_2_2));
        #endregion

        #region Public Methods
        /// <summary>
        ///     Determines whether this instance [can not modify parent scope data].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DataFlowException))]
        public void Can_not_modify_parent_scope_data()
        {
            var context = new DataContext
            {
                {data_bracket_0_0, "A"}
            };

            context.OpenNewLayer(string.Empty);
            context.Add(data_bracket_1_0, "B");

            context.Update(data_bracket_0_0, "B");

            context.CloseCurrentLayer();
        }

        /// <summary>
        ///     Determines whether this instance [can not remove parent scope data].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DataFlowException))]
        public void Can_not_remove_parent_scope_data()
        {
            var context = new DataContext
            {
                {data_bracket_0_0, "A"}
            };

            context.OpenNewLayer(string.Empty);
            context.Add(data_bracket_1_0, "B");

            context.Remove(data_bracket_0_0);

            context.CloseCurrentLayer();
        }

        /// <summary>
        ///     Determines whether this instance [can remove from current scope data].
        /// </summary>
        [TestMethod]
        public void Can_remove_from_current_scope_data()
        {
            var context = new DataContext();

            context.OpenNewLayer(string.Empty);
            context.Add(data_bracket_1_1, "C");
            context.Remove(data_bracket_1_1);
            context.CloseCurrentLayer();
        }

        [TestMethod]
        [ExpectedException(typeof(DataFlowException))]
        public void Forwarded_keys_cannot_be_modify()
        {
            var keyA = new DataKey<string>("A");
            var keyB = new DataKey<string>("B");

            var context = new DataContext
            {
                {keyA, "A"}
            };
            context.ForwardKey(keyB, keyA);

            context.Contains(keyA).Should().BeTrue();
            context.Contains(keyB).Should().BeTrue();

            context.Add(keyB, "B");
        }

        /// <summary>
        ///     Shoulds the math layers.
        /// </summary>
        [TestMethod]
        public void Should_get_data_when_key_is_forwarded()
        {
            var context = new DataContext
            {
                {data_bracket_0_0, "A"}
            };

            context.Contains(data_bracket_1_0).Should().BeFalse();

            context.ForwardKey(data_bracket_1_0, data_bracket_0_0);

            context.Contains(data_bracket_1_0).Should().BeTrue();

            context.Get(data_bracket_0_0).Should().Be("A");
            context.Get(data_bracket_1_0).Should().Be("A");
            context.TryGet(data_bracket_1_0).Should().Be("A");
        }

        /// <summary>
        ///     Shoulds the math layers.
        /// </summary>
        [TestMethod]
        public void Should_math_layers()
        {
            var dataContext = new DataContext();
            dataContext.OpenNewLayer(string.Empty);
            dataContext.OpenNewLayer(string.Empty);
            dataContext.OpenNewLayer(string.Empty);

            dataContext.CloseCurrentLayer();
            dataContext.CloseCurrentLayer();
            dataContext.CloseCurrentLayer();
            dataContext.CloseCurrentLayer();
        }

        /// <summary>
        ///     Shoulds the remove all elements in bracket when bracket is closed.
        /// </summary>
        [TestMethod]
        public void Should_Remove_All_Elements_In_Bracket_When_Bracket_Is_Closed()
        {
            var context = new DataContext();

            context.Add(data_bracket_0_0, "A");

            context.OpenNewLayer(string.Empty);
            context.Add(data_bracket_1_0, "B");
            context.Add(data_bracket_1_1, "C");

            context.Get(data_bracket_0_0).Should().Be("A");
            context.Get(data_bracket_1_0).Should().Be("B");
            context.Get(data_bracket_1_1).Should().Be("C");

            context.OpenNewLayer(string.Empty);
            context.Add(data_bracket_2_0, "2_0");
            context.Add(data_bracket_2_1, "2_1");
            context.Add(data_bracket_2_2, "2_2");

            context.Get(data_bracket_0_0).Should().Be("A");
            context.Get(data_bracket_1_0).Should().Be("B");
            context.Get(data_bracket_1_1).Should().Be("C");

            context.Get(data_bracket_2_0).Should().Be("2_0");
            context.Get(data_bracket_2_1).Should().Be("2_1");
            context.Get(data_bracket_2_2).Should().Be("2_2");

            context.CloseCurrentLayer();

            context.Get(data_bracket_0_0).Should().Be("A");
            context.Get(data_bracket_1_0).Should().Be("B");
            context.Get(data_bracket_1_1).Should().Be("C");

            context.TryGet(data_bracket_2_0).Should().Be(null);
            context.TryGet(data_bracket_2_1).Should().Be(null);
            context.TryGet(data_bracket_2_2).Should().Be(null);

            context.CloseCurrentLayer();

            context.Get(data_bracket_0_0).Should().Be("A");

            context.CloseCurrentLayer();

            context.TryGet(data_bracket_0_0).Should().Be(null);
        }

        /// <summary>
        ///     Throws the error when layer is not matched.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DataFlowException))]
        public void Throw_Error_When_Layer_Is_Not_Matched()
        {
            var context = new DataContext();
            context.OpenNewLayer(string.Empty);
            context.OpenNewLayer(string.Empty);
            context.OpenNewLayer(string.Empty);

            context.CloseCurrentLayer();
            context.CloseCurrentLayer();
            context.CloseCurrentLayer();

            context.CloseCurrentLayer();
            context.CloseCurrentLayer();
        }
        #endregion
    }
}