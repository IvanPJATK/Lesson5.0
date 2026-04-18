using Lesson5.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lesson5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Reservation>> GetAll([FromQuery] DateOnly? Date, [FromQuery] string? Status, [FromQuery] int? RoomId)
        {
            var reservations = Database.DataStore.Reservations.AsEnumerable();

            if (Date.HasValue)
            {
                reservations = reservations.Where(r => r.Date.Equals(Date));
            }
            if (!string.IsNullOrWhiteSpace(Status))
            {
                reservations = reservations.Where(r => r.Status.Equals(Status));
            }
            if (RoomId.HasValue)
            {
                reservations = reservations.Where(r => r.RoomId.Equals(RoomId));
            }

            return Ok(reservations.ToList());
        }
        [HttpGet("{id:int}")]
        public ActionResult<Reservation> GetById(int id)
        {
            var reservation = Database.DataStore.Reservations.FirstOrDefault(r => r.Id  == id);
            if (reservation == null)
            {
                return NotFound($"Reservation with id {id} was not found");
            }
            return reservation;
        }
        [HttpPost]
        public ActionResult<Reservation> CreateReservation(Reservation reservation)
        {
            var isOverlapping = Database.DataStore.Reservations
                .Any(r => r.RoomId == reservation.RoomId && r.Date == reservation.Date
                && ((reservation.StartTime >= r.StartTime && reservation.StartTime < r.EndTime) || (reservation.EndTime > r.StartTime && reservation.EndTime <= r.EndTime)));
            if (isOverlapping)
            {
                return Conflict("This room is already booked for the selected time.");
            }

            reservation.Id = Database.DataStore.NextReservationId;
            Database.DataStore.Reservations.Add(reservation);

            return CreatedAtAction(nameof(GetById), new {id = reservation.Id}, reservation);
        }
        [HttpPut("{id:int}")]
        public ActionResult<Reservation> UpdateReservation(int id, Reservation reservation)
        {
            if(!id.Equals(reservation.Id)) { return BadRequest("Id mismatch"); }
            
            var resToUpdate = Database.DataStore.Reservations.FirstOrDefault(r => r.Id.Equals(id));
            if(resToUpdate == null) { return NotFound($"Reservation with id {id} doesn't exist."); }
            
            resToUpdate.OrganizerName = reservation.OrganizerName;
            resToUpdate.Status = reservation.Status;
            resToUpdate.Topic = reservation.Topic;
            resToUpdate.RoomId = reservation.RoomId;
            resToUpdate.Date = reservation.Date;
            resToUpdate.StartTime = reservation.StartTime;
            resToUpdate.EndTime = reservation.EndTime;

            return Ok(resToUpdate);
        }
        [HttpDelete("{id:int}")]
        public ActionResult<Reservation> DeleteReservation(int id)
        {
            var resToDelete = Database.DataStore.Reservations.FirstOrDefault(r =>r.Id.Equals(id));
            if (resToDelete == null)
            {
                return NotFound($"Reservation with id {id} doesn't exist.");
            }
            Database.DataStore.Reservations.Remove(resToDelete);

            return NoContent();
        }
    }
}
