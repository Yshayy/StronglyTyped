using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CaseClass;

namespace CaseClassTests
{
    [TestClass]
    public class UnitTest1
    {
        public class FirstName : Typed<FirstName, string>{}
        public class LastName : Typed<LastName, string> { }

        public class Leaf : Typed<Leaf, int>{}

        public class Node : Typed<Node, Tree>{}

        public class Tree : TypedUnion<Tree, Leaf, Node> { }

        public class IntOrBool : TypedUnion<IntOrBool, int, bool> { }

        [TestMethod]
        public void TestMethod1()
        {
            var intOrBool = IntOrBool.Of(true);
            var t = intOrBool.Case((int x) => 5)
                             .Case(x=> 10);

            Assert.AreEqual(t, 10);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var tree = Tree.Of(Node.Of(Tree.Of(Leaf.Of(5))))

            var name = FirstName.Of("yshay") + LastName.Of("Yaacobi");

        }
    }
}
