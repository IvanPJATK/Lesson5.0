using Lesson5.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lesson5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Room>> GetAll()
        {
            var rooms = Database.DataStore.Rooms.AsEnumerable();
            return Ok(rooms.ToList());
        }
        [HttpGet]
        public ActionResult<List<Room>> GetAll([FromQuery] int? minCapacity)
        {
            var rooms = Database.DataStore.Rooms.AsEnumerable();
            
            if(minCapacity.HasValue)
            {
                rooms = rooms.Where(r =>  r.Capacity >= minCapacity.Value);
            }

            return Ok(rooms.ToList());
        }
        [HttpGet("{id:int}")]
        public ActionResult<Room> GetById(int id)
        {
            var room = Database.DataStore.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
            {
                return NotFound($"Room with id {id} was not found");
            }
            return Ok(room);
        }
        [HttpGet("building/{buildingCode}")]
        public ActionResult<List<Room>> GetByBuilding(string buildingCode)
        {
            var rooms = Database.DataStore.Rooms
                .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase)).ToList();
            
            return Ok(rooms);
        }
        [HttpPost]
        public ActionResult<Room> CreateRoom(Room room)
        {
            room.Id = Database.DataStore.NextRoomId;
            Database.DataStore.Rooms.Add(room);

            return CreatedAtAction(nameof(GetById), new {id = room.Id }, room);
        }

    }
}
