using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ToDoGrpc.Data;
using ToDoGrpc.Models;

namespace ToDoGrpc.Services;

public class ToDoService : ToDoIt.ToDoItBase
{
    private readonly AppDbContext appDbContext;

    public ToDoService(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
    {
        if(string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        var todoItem = new ToDoItem
        {
            Title = request.Title,
            Description = request.Description
        };

        await appDbContext.AddAsync(todoItem);
        await appDbContext.SaveChangesAsync();

        return new CreateToDoResponse()
        {
            Id = todoItem.Id
        };  
    }

    public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
    {
        var item = await appDbContext.ToDoItems.SingleOrDefaultAsync(c=> c.Id == request.Id) ?? throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));

        return new ReadToDoResponse()
        {
            Description = item.Description,
            Id = item.Id,
            Title = item.Title,
            ToDoStatus = item.ToDoStatus
        };
    }

    public override async Task<GetAllResponse> ListToDto(GetAllRequest request, ServerCallContext context)
    {
        var response =  new GetAllResponse();
        var items = await appDbContext.ToDoItems.ToListAsync();

        response.ToDo.AddRange(items.Select(c=> new ReadToDoResponse()
        {
            Description = c.Description,
            Id = c.Id,
            Title = c.Title,
            ToDoStatus = c.ToDoStatus
        }));

        return response;
    }

    public override async Task<UpdateToDoResponse> UpdateToDto(UpdateToDoRequest request, ServerCallContext context)
    {
        var item = await appDbContext.ToDoItems.SingleOrDefaultAsync(c=> c.Id == request.Id) ?? throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));

        if(string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        item.Title = request.Title;
        item.Description = request.Description;
        item.ToDoStatus = request.ToDoStatus;

        appDbContext.ToDoItems.Update(item);

        await appDbContext.SaveChangesAsync();

        return new UpdateToDoResponse
        {
             Id = item.Id
        };
    }

    public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
    {
        var item = await appDbContext.ToDoItems.SingleOrDefaultAsync(c=> c.Id == request.Id) ?? throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));

        appDbContext.ToDoItems.Remove(item);

        await appDbContext.SaveChangesAsync();

        return new DeleteToDoResponse()
        {
             Id = item.Id
        };
    }
}