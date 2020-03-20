using Microsoft.AspNetCore.SignalR;
using NeuroMan.Models;
using NeuroMan.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class NeuroHub : Hub
    {
        private readonly RoomService roomService;
        public NeuroHub(RoomService roomService)
        {
            this.roomService = roomService;
        }

        public override async Task OnConnectedAsync()
        {
            await AddToGroup(Context.GetHttpContext().Request.Cookies["room"]);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromGroup(Context.GetHttpContext().Request.Cookies["room"]);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            Room room = roomService.GetRoom(groupName);
            room.Join(Context.GetHttpContext().Request.Cookies["name"], Context.GetHttpContext().Connection.RemoteIpAddress.ToString());
            await Clients.Caller.SendAsync("SetCase", room.neuralNetwork.GetInputValues());
            await Clients.Groups(groupName).SendAsync("UpdateUsersStates", room.GetParticipants().Values.ToList());
        }

        private async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            Room room = roomService.GetRoom(groupName);
            room.Unjoin(Context.GetHttpContext().Request.Cookies["name"]);
            roomService.DeleteFreeRooms();
            await Clients.Groups(groupName).SendAsync("UpdateUsersStates", room.GetParticipants().Values.ToList());
        }

        public async Task SetWeights(bool readiness, List<double> inputWeights, List<double> outputWeights)
        {
            Room room = roomService.GetRoom(Context.GetHttpContext().Request.Cookies["room"]);

            if (readiness)
                room.neuralNetwork.SetWeights(Context.GetHttpContext().Request.Cookies["name"], inputWeights, outputWeights);

            bool already = room.ChangeReadiness(Context.GetHttpContext().Request.Cookies["name"]);

            if (already) await CalculateOutput(room);


            await Clients.Groups(room.Name).SendAsync("UpdateUsersStates", room.GetParticipants().Values.ToList());
        }

        private async Task SendCase(Room room)
        {
            await Clients.Groups(room.Name).SendAsync("SetCase", room.neuralNetwork.GetInputValues());
        }

        private async Task CalculateOutput(Room room)
        {
            foreach(Participant p in room.GetParticipants().Values)
            {
                p.IsReady = false;
            }

            room.neuralNetwork.GenerateInput();
            await SendCase(room);

            await Clients.Groups(room.Name).SendAsync("ShowAnswer", room.neuralNetwork.Answer);
            await Clients.Groups(room.Name).SendAsync("UpdateUsersStates", room.GetParticipants().Values.ToList());
        }
    }
}