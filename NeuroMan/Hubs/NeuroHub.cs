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
        RoomService roomService;
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

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            Room room = roomService.GetRoom(groupName);
            room.Join(Context.GetHttpContext().Connection.RemoteIpAddress.ToString());
            List<Participant> participants = room.GetParticipants().Values.ToList();
            await Clients.Groups(groupName).SendAsync("UpdateUsersStates", participants);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            Room room = roomService.GetRoom(groupName);
            room.Unjoin(Context.GetHttpContext().Connection.RemoteIpAddress.ToString());
            await Clients.Groups(groupName).SendAsync("UpdateUsersStates", room.GetParticipants().Values);
        }

        public async Task SendWeights(List<double> inputWeights, List<double> outputWeights)
        {
            Room room = roomService.GetRoom(Context.GetHttpContext().Request.Cookies["room"]);
            Participant caller = room.GetParticipants()[Context.GetHttpContext().Connection.RemoteIpAddress.ToString()];
            caller.inputWeights = inputWeights;
            caller.outputWeights = outputWeights;
            bool already = room.SetReady(Context.GetHttpContext().Connection.RemoteIpAddress.ToString());
            if (already) await CalculateOutput(room);
            await Clients.Groups(room.Name).SendAsync("UpdateUsersStates", room.GetParticipants().Values);
        }

        public async Task LoadWeights()
        {
            Room room = roomService.GetRoom(Context.GetHttpContext().Request.Cookies["room"]);
            Participant caller = room.GetParticipants()[Context.GetHttpContext().Connection.RemoteIpAddress.ToString()];

            await Clients.Caller.SendAsync("UpdateWeigths", caller.inputWeights, caller.outputWeights);
        }

        private async Task CalculateOutput(Room room)
        {
            foreach(Participant p in room.GetParticipants().Values)
            {
                p.isReady = false;
            }

            await Clients.Groups(room.Name).SendAsync("ShowAnswer", room.answer);
            await Clients.Groups(room.Name).SendAsync("UpdateUsersStates", room.GetParticipants().Values);
        }
    }
}