//using BlazorBankingApplication.Models;
//using Microsoft.AspNetCore.Components;
//using Org.BouncyCastle.Bcpg.OpenPgp;
//using System.Linq.Expressions;

//namespace BlazorBankingApplication.Components.Pages
//{
//    public partial class Home
//    {
//        List<AppointmentModel> appointments;
//        List<AvailableAppointmentModel> availableAppointments;
//        List<AvailableAppointmentModel> thisAppointment;

//        private AppointmentModel newAppointment = new AppointmentModel();

//        //To Track  the data a user selects 
//        object dateValue;

//        //To Track the date and time that a user wants to reserve
//        DateTime selectedDateTime;

//        //Id that will be used to keep the track for the currently selected appointment
//        //of available appointments
//        int selectedAvailableId;

//        //Booleans for changing the UI
//        bool stepTwoReady = false;
//        bool stepThreeReady = false;
//        bool noAppointments = false;

//        bool toggle = false;

//        //Load the Initial data
//        protected override async Task OnInitializedAsync()
//        {
//            string sql = "SELECT * FROM appointment";
//            appointments = await _data.LoadData<AppointmentModel, dynamic>(sql, new { },
//                _config.GetConnectionString("MySQLConnection"));
//        }

//        private async Task ShowAvailableAppointments(ChangeEventArgs args)
//        {
//            dateValue = args.Value;
//            string sql = "SELECT * FROM available_appointment WHERE StartDate = @date;";
//            availableAppointments = await _data.LoadData<AvailableAppointmentModel, dynamic>(sql, new { date = @dateValue },
//                _config.GetConnectionString("MySQLConnection"));

//            availableAppointments = availableAppointments.OrderBy(x => x.StartTime).ToList();

//            stepTwoReady = true;

//            if (availableAppointments.Count == 0)
//            {
//                noAppointments = true;
//            }
//            else noAppointments = false;
//        }

//        private async Task SelectAppointment(DateTime dateTime, int availableId)
//        {
//            selectedDateTime = dateTime;
//            newAppointment.StartTime = dateTime;
//            stepThreeReady = true;
//            selectedAvailableId = availableId;

//            string sql = "SELECT * FROM available_appointment WHERE Id=@Id;";
//            thisAppointment = await _data.LoadData<AvailableAppointmentModel, dynamic>(sql, new { Id = availableId },
//                _config.GetConnectionString("MySQLConnection"));

//            newAppointment.EndTime = thisAppointment[0].EndTime;
//        }

//        private async Task SaveAppointment()
//        {
//            AppointmentModel a = new AppointmentModel
//            {
//                AvailableAppointmentId = selectedAvailableId,
//                FullName = newAppointment.FullName,
//                Reason = newAppointment.Reason,
//                StartTime = newAppointment.StartTime.ToUniversalTime(),
//                EndTime = newAppointment.EndTime.ToUniversalTime(),
//                CreatedAt = DateTime.UtcNow,
//                UpdatedAt = DateTime.UtcNow,
//                UpdatedBy = "Imported",
//                Active = true
//            };

//            string sql = "INSERT INTO appointment (AvailableAppointmentId, FullName, Reason, StartTime, EndTime, CreatedAt," +
//                "UpdatedAt, UpdatedBy, Active) VALUES (@AvailableAppointmentId, @FullName, @Reason, @StartTime, @EndTime, @CreatedAt," +
//            "@UpdatedAt, @UpdatedBy, @Active);";

//            await _data.SaveData(sql, a, _config.GetConnectionString("MySQLConnection"));

//            await ReserveAvailableAppointment();

//            stepTwoReady = false;
//            stepThreeReady = false;

//            //reset the date input
//            toggle = !toggle;

//            //reset newAppointment
//            newAppointment = new AppointmentModel();

//            await OnInitializedAsync();
//        }

//        private async Task ReserveAvailableAppointment()
//        {
//            string sql = "UPDATE available_appointment SET Reserved = true WHERE Id = @Id";
//            await _data.SaveData(sql, new { Id = selectedAvailableId }, _config.GetConnectionString("MySQLConnection"));
//            await OnInitializedAsync();
//        }

//        private async Task DeleteAppointment(int id, int cancelId)
//        {
//            string sql = "DELETE FROM appointment WHERE Id=@Id";
//            await _data.SaveData(sql, new { Id = id }, _config.GetConnectionString("MySQLConnection"));

//            await CancelReservationForAvailableAppointment(cancelId);

//            //reset the date input 
//            toggle = !toggle;
//            stepTwoReady = false;
//            stepThreeReady = false;

//            await OnInitializedAsync();
//        }

//        private async Task CancelReservationForAvailableAppointment(int cancelId)
//        {
//            string sql = "UPDATE available_appointment SET Reserved = false WHERE Id = @Id";
//            await _data.SaveData(sql, new { Id = cancelId }, _config.GetConnectionString("MySQLConnection"));
//            await OnInitializedAsync();
//        }
//    }
//}
