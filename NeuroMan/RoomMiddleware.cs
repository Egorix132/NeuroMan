using Microsoft.AspNetCore.Http;
using NeuroMan.Models;
using NeuroMan.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RoomMiddleware
{
    private readonly RequestDelegate _next;
    RoomService roomService;

    public RoomMiddleware(RequestDelegate next, RoomService roomService)
    {
        this._next = next;
        this.roomService = roomService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string room;
        if (context.Request.Query.ContainsKey("room") && !context.Request.Cookies.ContainsKey("room"))
        {
            room = context.Request.Query["room"];
            context.Response.Cookies.Append("room", room);
            context.Items["room"] = room;
        }
        else if(!context.Request.Cookies.ContainsKey("room") || roomService.GetRoom(context.Request.Cookies["room"]) == null)
        {
            room = FindRoom();
            context.Response.Cookies.Append("room", room);
            context.Items["room"] = room;
        }
        else
        {
            room = context.Request.Cookies["room"];
        }
        context.Items["room"] = room;
        await _next.Invoke(context);
    }

    private string FindRoom()
    {
        string roomName = roomService.FindRoom();

        if(roomName != null) 
            return roomName;

        roomName = roomService.AddRoom();
        return roomName;
    }
}