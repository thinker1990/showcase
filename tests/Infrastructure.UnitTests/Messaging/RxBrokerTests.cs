using Abstractions.Messaging;
using Infrastructure.Messaging;

namespace Infrastructure.UnitTests.Messaging;

internal sealed class RxBrokerTests
{
    private const string Topic = "topic";
    private const string Message = "message";
    private RxBroker _broker = default!;

    [SetUp]
    public void Setup() => _broker = new RxBroker();

    [TestCase(WellKnown.Topic.SwitchLanguage, WellKnown.Message.ChineseSimplified)]
    [TestCase("PeerToPeer:LightControl", """{"Channel": "1","Action": "On","Brightness": "50"}""")]
    public void SubscribedMessageContentShouldBeTheSameAsPublished(string topic, string message)
    {
        var received = new List<string>();

        using var _ = _broker.Subscribe(topic).Subscribe(received.Add);
        _broker.Publish(topic, message);

        received.Should().ContainSingle(it => it == message);
    }

    [TestCase(5)]
    [TestCase(42)]
    public void SubscribedMessageCountShouldBeTheSameAsPublished(int count)
    {
        var received = new List<string>();

        using var _ = _broker.Subscribe(Topic).Subscribe(received.Add);
        for (var i = 0; i < count; i++)
        {
            _broker.Publish(Topic, Message);
        }

        received.Should().HaveCount(count);
    }

    [TestCase("")]
    [TestCase("   ")]
    public void ShouldThrowExceptionWhenPublishEmptyTopic(string topic)
    {
        var publish = () => _broker.Publish(topic, Message);

        publish.Should().Throw<ArgumentException>();
    }

    [TestCase("")]
    [TestCase("   ")]
    public void ShouldThrowExceptionWhenPublishEmptyMessage(string message)
    {
        var publish = () => _broker.Publish(Topic, message);

        publish.Should().Throw<ArgumentException>();
    }

    [TestCase("")]
    [TestCase("   ")]
    public void ShouldThrowExceptionWhenSubscribeEmptyTopic(string topic)
    {
        var publish = () => _broker.Subscribe(topic);

        publish.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ShouldReceiveNothingIfSubscribeMismatchedTopic()
    {
        var received = new List<string>();
        var (topicA, topicB) = ("TopicA", "TopicB");

        using var _ = _broker.Subscribe(topicA).Subscribe(received.Add);
        _broker.Publish(topicB, Message);

        received.Should().BeEmpty();
    }

    [Test]
    public void ShouldMissTheMessageWhenSubscribeAfterItHasPublished()
    {
        var received = new List<string>();

        _broker.Publish(Topic, Message);
        using var _ = _broker.Subscribe(Topic).Subscribe(received.Add);

        received.Should().BeEmpty();
    }

    [Test]
    public void ShouldReceiveSameMessagesByMultipleSubscribers()
    {
        var received1 = new List<string>();
        var received2 = new List<string>();

        using var subscription1 = _broker.Subscribe(Topic).Subscribe(received1.Add);
        using var subscription2 = _broker.Subscribe(Topic).Subscribe(received2.Add);
        _broker.Publish(Topic, Message);

        received1.Should().BeEquivalentTo(received2);
    }

    [TearDown]
    public void TearDown() => _broker.Dispose();
}