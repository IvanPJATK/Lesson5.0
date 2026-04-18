using Lesson5.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lesson5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        //[HttpGet]
        //public ActionResult<List<Room>> GetAll()
        //{
        //    var rooms = Database.DataStore.Rooms.AsEnumerable();
        //    return Ok(rooms.ToList());
        //}
        [HttpGet]
        public ActionResult<List<Room>> GetAll([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool? activeOnly)
        {
            var rooms = Database.DataStore.Rooms.AsEnumerable();

            if (minCapacity.HasValue)
            {
                rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
            }
            if (hasProjector.HasValue)
            {
                rooms = rooms.Where(r => r.HasProjector.Equals(hasProjector));
            }
            if (activeOnly.HasValue)
            {
                rooms = rooms.Where(r => r.IsActive.Equals(activeOnly));
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

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }
        [HttpPut("{id:int}")]
        public ActionResult<Room> UpdateRoom(int id, Room room)
        {
            if(id != room.Id) { return BadRequest("Id mismatch"); }
            
            var existingRoom = Database.DataStore.Rooms.FirstOrDefault(r => r.Id == id);

            if (existingRoom == null) { return NotFound($"Room with id {id} was not found"); }

            existingRoom.Name = room.Name;
            existingRoom.BuildingCode = room.BuildingCode;
            existingRoom.HasProjector = room.HasProjector;
            existingRoom.IsActive = room.IsActive;
            existingRoom.Floor = room.Floor;
            existingRoom.Capacity = room.Capacity;

            return Ok(existingRoom);
        }
        [HttpDelete("{id:int}")]
        public ActionResult<Room> DeleteRoom(int id)
        {
            var existingRoom = Database.DataStore.Rooms.FirstOrDefault(r => r.Id == id);

            if (existingRoom == null) { return NotFound($"Room with id {id} was not found"); }

            Database.DataStore.Rooms.Remove(existingRoom);

            return NoContent();
        }
    }
}
