using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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
        static readonly DataKey<string> data_bracket_0_0 = new DataKey<string>(typeof(DataFlowTests), nameof(data_bracket_0_0));

        /// <summary>
        ///     The data bracket 1 0
        /// </summary>
        static readonly DataKey<string> data_bracket_1_0 = new DataKey<string>(typeof(DataFlowTests), nameof(data_bracket_1_0));

        /// <summary>
        ///     The data bracket 1 1
        /// </summary>
        static readonly DataKey<string> data_bracket_1_1 = new DataKey<string>(typeof(DataFlowTests), nameof(data_bracket_1_1));

        /// <summary>
        ///     The data bracket 2 0
        /// </summary>
        static readonly DataKey<string> data_bracket_2_0 = new DataKey<string>(typeof(DataFlowTests), nameof(data_bracket_2_0));

        /// <summary>
        ///     The data bracket 2 1
        /// </summary>
        static readonly DataKey<string> data_bracket_2_1 = new DataKey<string>(typeof(DataFlowTests), nameof(data_bracket_2_1));

        /// <summary>
        ///     The data bracket 2 2
        /// </summary>
        static readonly DataKey<string> data_bracket_2_2 = new DataKey<string>(typeof(DataFlowTests), nameof(data_bracket_2_2));
        #endregion

        #region Public Methods
        /// <summary>
        ///     Determines whether this instance [can not modify parent scope data].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DataFlowException))]
        public void Can_not_modify_parent_scope_data()
        {
            var context = new DataContext();
            context.Add(data_bracket_0_0, "A");

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
            var context = new DataContext();
            context.Add(data_bracket_0_0, "A");

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

            context.OpenNewLayer("Layer Q");
            context.Add(data_bracket_1_1, "C");

            new DataContextDebugView(context).Items[1].Items.Length.Should().Be(1);

            context.Remove(data_bracket_1_1);
            context.CloseCurrentLayer();
        }

        [TestMethod]
        public void CanBeSerialize()
        {
            var context = new DataContext();

            var key1 = new DataKey<string>(typeof(DataFlowTests), "key1");
            var key2 = new DataKey<string>(typeof(DataFlowTests), "key2");
            var key3 = new DataKey<string>(typeof(DataFlowTests), "key3");
            var key4 = new DataKey<string>(typeof(DataFlowTests), "key4");

            context.Add(key1, "A");
            context.Add(key2, "B");
            context.OpenNewLayer("Layer1");
            context.Add(key3, "C");
            context.Add(key4, "D");

            var json = JsonConvert.SerializeObject(context, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            File.WriteAllText("D:\\A.txt", json);

            context = JsonConvert.DeserializeObject<DataContext>(json);

            context.CurrentLayerName.Should().Be("Layer1");

            context.Get(key4).Should().Be("D");

            context.CloseCurrentLayer();

            context.Contains(key4).Should().BeFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(DataFlowException))]
        public void Forwarded_keys_cannot_be_modify()
        {
            var keyA = new DataKey<string>(typeof(DataFlowTests), "A");
            var keyB = new DataKey<string>(typeof(DataFlowTests), "B");

            var context = new DataContext();
            context.Add(keyA , "A");
            context.ForwardKey(keyB, keyA);

            context.Contains(keyA).Should().BeTrue();
            context.Contains(keyB).Should().BeTrue();

            context.Add(keyB, "B");
        }

        [TestMethod]
        public void Should_Call_Action_On_Insert_Operation()
        {
            var context = new DataContext();

            var temp = string.Empty;

            context.OnInsert(data_bracket_0_0, () => { temp = "A"; });

            context.Add(data_bracket_1_0, string.Empty);

            temp.Should().BeEmpty();

            context.Add(data_bracket_0_0, string.Empty);

            temp.Should().Be("A");
        }

        [TestMethod]
        public void Should_Call_Action_On_Update_Operation()
        {
            var context = new DataContext();

            var temp = string.Empty;

            context.OnUpdate(data_bracket_0_0, () => { temp = "A"; });

            context.Add(data_bracket_1_0, string.Empty);
            context.Update(data_bracket_1_0, string.Empty);

            temp.Should().BeEmpty();

            context.Add(data_bracket_0_0, string.Empty);
            temp.Should().BeEmpty();

            context.Update(data_bracket_0_0, string.Empty);
            temp.Should().Be("A");

            context.OnRemove(data_bracket_0_0, () => { temp = "R"; });

            context.Remove(data_bracket_0_0);
            temp.Should().Be("R");
        }

        [TestMethod]
        public void Should_fire_events_when_layer_closed()
        {
            var context = new DataContext();

            var key1 = new DataKey<string>(typeof(DataFlowTests), "key1");
            var key2 = new DataKey<string>(typeof(DataFlowTests), "key2");
            var key3 = new DataKey<string>(typeof(DataFlowTests), "key3");
            var key4 = new DataKey<string>(typeof(DataFlowTests), "key4");

            var temp = "";
            context.OnRemove(key1, () => temp = "key1_remoed");
            context.OpenNewLayer("L1");
            temp.Should().Be(string.Empty);
            context.Add(key1, "A");
            context.Add(key2, "B");

            context.CloseCurrentLayer();

            temp.Should().Be("key1_remoed");
        }

        /// <summary>
        ///     Shoulds the math layers.
        /// </summary>
        [TestMethod]
        public void Should_get_data_when_key_is_forwarded()
        {
            var context = new DataContext();
            context.Add(data_bracket_0_0, "A");

            context.Contains(data_bracket_1_0).Should().BeFalse();

            context.ForwardKey(data_bracket_1_0, data_bracket_0_0);

            context.Contains(data_bracket_1_0).Should().BeTrue();

            context.Get(data_bracket_0_0).Should().Be("A");
            context.Get(data_bracket_1_0).Should().Be("A");
            context.TryGet(data_bracket_1_0).Should().Be("A");
        }

        [TestMethod]
        public void Should_get_data_when_virtual_key_used()
        {
            var context = new DataContext();

            var key1 = new DataKey<string>(typeof(DataFlowTests), "key1");
            var key2 = new DataKey<string>(typeof(DataFlowTests), "key2");
            var key3 = new DataKey<string>(typeof(DataFlowTests), "key3");
            var key4 = new DataKey<string>(typeof(DataFlowTests), "key4");

            context.Add(key1, "A");
            context.Add(key2, "B");

            context.Contains(key3).Should().BeFalse();

            context.SetupGet(key3, c => c.Get(key1) + "X" + c.Get(key2));

            context.Get(key3).Should().Be("AXB");

            context.ForwardKey(key4, key3);

            context.Get(key4).Should().Be("AXB");
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