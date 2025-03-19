using System;
using Contracts;
using MassTransit;

namespace AuctionService.Consumer;

public class AuctionCreatedFaultsConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("----> AuctionCreatedFaultsConsumer is consuming the fault: ");

        var exception = context.Message.Exceptions.First();

        if(exception.ExceptionType == "System.ArgumentException")
        {
            context.Message.Message.Model = "FooBar";
            await context.Publish(context.Message.Message);
        }
        else
        {
            Console.WriteLine("Not an argument exception - udpate error dashboard");
        }
    }
}
