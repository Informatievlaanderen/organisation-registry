namespace OrganisationRegistry.UnitTests;

using System;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using OrganisationRegistry.Security;
using Xunit;

public class TreeTests
{
    /*
        Use cases from projections:
        - Somebody adds an organisation
        - Somebody adds an organisation with a parent
        - Somebody adds an organisation with a parent, which also has a tree
        - Somebody changes the parent of an organisation
        - Somebody changes the parent of an organisation, which also has a tree
        - Somebody removes a parent of an organisation

        - Serialise the tree
    */

    [Fact]
    public void AddNewOrganisation()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();

        // act
        tree.AddNode(new OvoNumber("OVO1"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(1);
        changes[0].Id.Should().Be("OVO1");
        changes[0].Traverse().ToSeparatedList().Should().Be("OVO1");
    }

    [Fact]
    public void AddNewOrganisationWithParent()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AcceptChanges();

        // act
        tree.AddNode(new OvoNumber("OVO2"), new OvoNumber("OVO1"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(2);

        changes[0].Id.Should().Be("OVO1");
        changes[0].Children.Count.Should().Be(1);
        changes[0].Children[0].Id.Should().Be("OVO2");
        changes[0].Traverse().ToSeparatedList().Should().Be("OVO1|OVO2");

        changes[1].Id.Should().Be("OVO2");
        changes[1].Traverse().ToSeparatedList().Should().Be("OVO2");
    }

    [Fact]
    public void AddNewOrganisationWithParentAndFamily()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO2"), new OvoNumber("OVO1"));
        tree.AcceptChanges();

        // act
        tree.AddNode(new OvoNumber("OVO3"), new OvoNumber("OVO2"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(3);

        changes[0].Id.Should().Be("OVO1");
        changes[0].Children.Count.Should().Be(1);
        changes[0].Children[0].Id.Should().Be("OVO2");
        changes[0].Traverse().ToSeparatedList().Should().Be("OVO1|OVO2|OVO3");

        changes[1].Id.Should().Be("OVO2");
        changes[1].Children.Count.Should().Be(1);
        changes[1].Children[0].Id.Should().Be("OVO3");
        changes[1].Traverse().ToSeparatedList().Should().Be("OVO2|OVO3");

        changes[2].Id.Should().Be("OVO3");
        changes[2].Traverse().ToSeparatedList().Should().Be("OVO3");
    }

    [Fact]
    public void UpdateOrganisationChangeParent()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO2"));
        tree.AddNode(new OvoNumber("OVO3"), new OvoNumber("OVO1"));
        tree.AcceptChanges();

        // act
        tree.ChangeNodeParent(new OvoNumber("OVO3"), new OvoNumber("OVO2"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(3);

        changes[0].Id.Should().Be("OVO1");
        changes[0].Children.Count.Should().Be(0);
        changes[0].Traverse().ToSeparatedList().Should().Be("OVO1");

        changes[1].Id.Should().Be("OVO2");
        changes[1].Children.Count.Should().Be(1);
        changes[1].Children[0].Id.Should().Be("OVO3");
        changes[1].Traverse().ToSeparatedList().Should().Be("OVO2|OVO3");

        changes[2].Id.Should().Be("OVO3");
        changes[2].Traverse().ToSeparatedList().Should().Be("OVO3");
    }

    [Fact]
    public void UpdateOrganisationChangeParentAndFamily()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO2"));
        tree.AddNode(new OvoNumber("OVO3"), new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO4"), new OvoNumber("OVO2"));
        tree.AcceptChanges();

        // act
        tree.ChangeNodeParent(new OvoNumber("OVO4"), new OvoNumber("OVO3"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(4);

        changes[0].Id.Should().Be("OVO1");
        changes[0].Children.Count.Should().Be(1);
        changes[0].Children[0].Id.Should().Be("OVO3");
        changes[0].Traverse().ToSeparatedList().Should().Be("OVO1|OVO3|OVO4");

        changes[1].Id.Should().Be("OVO2");
        changes[1].Children.Count.Should().Be(0);
        changes[1].Traverse().ToSeparatedList().Should().Be("OVO2");

        changes[2].Id.Should().Be("OVO3");
        changes[2].Children.Count.Should().Be(1);
        changes[2].Children[0].Id.Should().Be("OVO4");
        changes[2].Traverse().ToSeparatedList().Should().Be("OVO3|OVO4");

        changes[3].Id.Should().Be("OVO4");
        changes[3].Traverse().ToSeparatedList().Should().Be("OVO4");
    }

    [Fact]
    public void UpdateOrganisationChangeParentAndFamily2()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO2"));
        tree.AddNode(new OvoNumber("OVO3"), new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO4"), new OvoNumber("OVO3"));
        tree.AddNode(new OvoNumber("OVO5"), new OvoNumber("OVO4"));
        tree.AcceptChanges();

        // act
        tree.ChangeNodeParent(new OvoNumber("OVO4"), new OvoNumber("OVO2"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(4);

        changes[0].Id.Should().Be("OVO1");
        changes[0].Children.Count.Should().Be(1);
        changes[0].Children[0].Id.Should().Be("OVO3");
        changes[0].Traverse().ToSeparatedList().Should().Be("OVO1|OVO3");

        changes[1].Id.Should().Be("OVO2");
        changes[1].Children.Count.Should().Be(1);
        changes[1].Children[0].Id.Should().Be("OVO4");
        changes[1].Traverse().ToSeparatedList().Should().Be("OVO2|OVO4|OVO5");

        changes[2].Id.Should().Be("OVO3");
        changes[2].Children.Count.Should().Be(0);
        changes[2].Traverse().ToSeparatedList().Should().Be("OVO3");

        changes[3].Id.Should().Be("OVO4");
        changes[3].Parent!.Id.Should().Be("OVO2");
        changes[3].Children.Count.Should().Be(1);
        changes[3].Children[0].Id.Should().Be("OVO5");
        changes[3].Traverse().ToSeparatedList().Should().Be("OVO4|OVO5");
    }

    [Fact]
    public void UpdateOrganisationChangeParentAndFamily3()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO2"));
        tree.AddNode(new OvoNumber("OVO3"), new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO4"), new OvoNumber("OVO3"));
        tree.AddNode(new OvoNumber("OVO5"), new OvoNumber("OVO4"));
        tree.AcceptChanges();

        // act
        tree.ChangeNodeParent(new OvoNumber("OVO2"), new OvoNumber("OVO1"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(2);

        changes[0].Id.Should().Be("OVO1");
        changes[0].Children.Count.Should().Be(2);
        var children = changes[0].Children.OrderBy(x => x.Id).ToList();
        children[0].Id.Should().Be("OVO2");
        children[1].Id.Should().Be("OVO3");
        changes[0].Traverse().ToSeparatedList().Should().Be("OVO1|OVO3|OVO4|OVO5|OVO2");

        changes[1].Id.Should().Be("OVO2");
        changes[1].Parent!.Id.Should().Be("OVO1");
        changes[1].Traverse().ToSeparatedList().Should().Be("OVO2");
    }

    [Fact]
    public void RemoveOrganisationParent()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO2"));
        tree.AddNode(new OvoNumber("OVO3"), new OvoNumber("OVO1"));
        tree.AcceptChanges();

        // act
        tree.RemoveNodeParent(new OvoNumber("OVO3"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(2);

        changes[0].Id.Should().Be("OVO1");
        changes[0].Children.Count.Should().Be(0);
        changes[0].Traverse().ToSeparatedList().Should().Be("OVO1");

        changes[1].Id.Should().Be("OVO3");
        changes[1].Parent.Should().BeNull();
        changes[1].Traverse().ToSeparatedList().Should().Be("OVO3");
    }

    [Fact]
    public void RemoveOrganisationParentAndFamily()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO2"), new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO3"), new OvoNumber("OVO2"));
        tree.AddNode(new OvoNumber("OVO4"), new OvoNumber("OVO3"));
        tree.AcceptChanges();

        // act
        tree.RemoveNodeParent(new OvoNumber("OVO3"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(3);

        changes[0].Id.Should().Be("OVO1");
        changes[0].Children.Count.Should().Be(1);
        changes[0].Children[0].Id.Should().Be("OVO2");
        changes[0].Traverse().ToSeparatedList().Should().Be("OVO1|OVO2");

        changes[1].Id.Should().Be("OVO2");
        changes[1].Children.Count.Should().Be(0);
        changes[1].Traverse().ToSeparatedList().Should().Be("OVO2");

        changes[2].Id.Should().Be("OVO3");
        changes[2].Children.Count.Should().Be(1);
        changes[2].Children[0].Id.Should().Be("OVO4");
        changes[2].Traverse().ToSeparatedList().Should().Be("OVO3|OVO4");
    }

    [Fact]
    public void RemoveOrganisationParentWhenThereIsNone()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AcceptChanges();

        // act
        tree.RemoveNodeParent(new OvoNumber("OVO1"));

        // assert
        var changes = tree.GetChanges().ToList();
        changes.Count.Should().Be(0);
    }

    [Fact(Skip = "Serialization with json doesnt work yet")]
    public void SerializeDeserializeTree()
    {
        // arrange
        ITree<OvoNumber> tree = new Tree<OvoNumber>();
        tree.AddNode(new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO2"), new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO3"), new OvoNumber("OVO2"));
        tree.AddNode(new OvoNumber("OVO4"), new OvoNumber("OVO3"));
        tree.AddNode(new OvoNumber("OVO5"), new OvoNumber("OVO2"));
        tree.AddNode(new OvoNumber("OVO6"), new OvoNumber("OVO1"));
        tree.AddNode(new OvoNumber("OVO7"), new OvoNumber("OVO6"));
        tree.AddNode(new OvoNumber("OVO8"), new OvoNumber("OVO6"));
        tree.AddNode(new OvoNumber("OVO9"), new OvoNumber("OVO6"));
        tree.AcceptChanges();

        // act
        var jsonTree = JsonConvert.SerializeObject(tree);
        var newTree = (Tree<OvoNumber>?)JsonConvert.DeserializeObject(jsonTree, typeof(Tree<OvoNumber>));

        // assert
        newTree.Should().NotBeNull();
        var treeLines = string.Join(Environment.NewLine, newTree!.DrawTree().Values);
        var newTreeLines = string.Join(Environment.NewLine, newTree.DrawTree().Values);
        newTreeLines.Should().Be(treeLines);
    }
}

public class OvoNumber : INodeValue
{
    public OvoNumber(string ovoNumber)
    {
        Id = ovoNumber;
    }

    public string Id { get; }
}
