using PlayPalMini.Common;
using PlayPalMini.Model;
using PlayPalMini.Repository.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// komentirano sta i kako na UserRepository
namespace PlayPalMini.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        public static string connectionString = "Data Source=VREMENSKISTROJ;Initial Catalog=PlayPalMini;Integrated Security=True";

        //-------------- GET ALL REVIEWS ---------------------------
        public async Task<(List<Review>, string)> GetAllAsync()
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Review", theConnection);
                    theConnection.Open();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Review> list = new List<Review>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Review review = new Review();
                            review.Id = reader.GetGuid(0);
                            review.Title = reader.GetString(1);
                            review.Comment = reader.GetString(2);
                            review.Rating = reader.GetInt32(3);
                            review.BoardGameId = reader.GetGuid(4);
                            review.CreatedBy = reader.GetString(5);
                            review.UpdatedBy = reader.GetString(6);
                            review.DateCreated = reader.GetDateTime(7);
                            review.DateUpdated = reader.GetDateTime(8);
                            review.RegisteredUserId = reader.GetGuid(9);

                            list.Add(review);
                        }
                        reader.Close();
                        return (list, "Success");
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
        //-------------- GET ONE REVIEW BY ID ---------------------------
        public async Task<(Review, string)> GetOneByIdAsync(Guid id)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Review WHERE Id = @id", theConnection);
                    cmd.Parameters.AddWithValue("@id", id);
                    theConnection.Open();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        Review review = new Review();

                        while (reader.Read())
                        {
                            review.Id = reader.GetGuid(0);
                            review.Title = reader.GetString(1);
                            review.Comment = reader.GetString(2);
                            review.Rating = reader.GetInt32(3);
                            review.BoardGameId = reader.GetGuid(4);
                            review.CreatedBy = reader.GetString(5);
                            review.UpdatedBy = reader.GetString(6);
                            review.DateCreated = reader.GetDateTime(7);
                            review.DateUpdated = reader.GetDateTime(8);
                            review.RegisteredUserId = reader.GetGuid(9);
                        }
                        reader.Close();
                        return (review, "Success");
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
        //------------------ CREATE REVIEW ---------------------
        public async Task<(bool, string)> CreateReviewAsync(Review review)
        {
            string authenticatedUser = System.Web.HttpContext.Current.User.Identity.Name;

            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    //---- dohvati userId --------                    
                    ClaimsPrincipal principal = System.Web.HttpContext.Current.User as ClaimsPrincipal;
                    var userIdClaimValue = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    Guid? userId = null;
                    if (!string.IsNullOrEmpty(userIdClaimValue))
                    {
                        userId = new Guid(userIdClaimValue);
                    }
                    //-----------------------------

                    SqlCommand cmd = new SqlCommand("INSERT INTO Review VALUES (@id, @title, @comment, @rating, @BoardGameId, @createdby, @updatedby, @timecreated, @timeupdated, @RegisteredUserId);", theConnection);

                    cmd.Parameters.AddWithValue("@id", review.Id = Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@title", review.Title);
                    cmd.Parameters.AddWithValue("@comment", review.Comment);
                    cmd.Parameters.AddWithValue("@rating", review.Rating);
                    cmd.Parameters.AddWithValue("@BoardGameId", review.BoardGameId); // u Postmanu rucno kopirat value za ovo u body da se zna za koju igru se ostavlja review
                    cmd.Parameters.AddWithValue("@createdby", review.CreatedBy = authenticatedUser);
                    cmd.Parameters.AddWithValue("@updatedby", review.UpdatedBy = "n/a");
                    cmd.Parameters.AddWithValue("@timecreated", review.DateCreated = DateTime.Now);
                    cmd.Parameters.AddWithValue("@timeupdated", review.DateUpdated = DateTime.Now);
                    if (userId.HasValue)
                    { cmd.Parameters.AddWithValue("@RegisteredUserId", review.RegisteredUserId = userId.Value); } // u Postmanu ako si logiran automatski ce staviti Id, ako nisi moras rucno kopirat, a do cega ne moze doci zbog zabrane u kontroleru
                    else
                    { cmd.Parameters.AddWithValue("@RegisteredUserId", review.RegisteredUserId); }

                    theConnection.Open();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return (true, "Added this new review!");
                    }
                    else
                    {
                        return (false, "Failed to create new review.");
                    }
                }
            }
            catch (Exception)
            {
                return (false, "Exception");
            }
        }
        //------------------ EDIT REVIEW ---------------------
        public async Task<(Review, string)> EditReviewAsync(Review review, Guid id)
        {
            string authenticatedUser = System.Web.HttpContext.Current.User.Identity.Name; // dohvaca username za mapiranje UpdatedBy

            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    // prvo dohvati review po zadanom id
                    SqlCommand cmdGet = new SqlCommand("SELECT * FROM Review WHERE Id = @id", theConnection);
                    cmdGet.Parameters.AddWithValue("@id", id);
                    theConnection.Open();

                    SqlDataReader reader = await cmdGet.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Close();

                        // ako review postoji, dohvati CreatedBy da mogu usporediti je li logirani user i autor tog reviewa
                        SqlCommand cmdCheck = new SqlCommand("SELECT CreatedBy FROM Review WHERE Id = @id", theConnection);
                        cmdCheck.Parameters.AddWithValue("@id", id);

                        object result = await cmdCheck.ExecuteScalarAsync();
                        if (result != null)
                        {
                            string createdBy = result.ToString();

                            if (createdBy != authenticatedUser) // ako nije autor
                            {
                                return (null, "Unauthorized: Only the creator of the review can edit it.");
                            }

                            // ako je logirani user i autor, slobodno editiraj
                            SqlCommand cmd = new SqlCommand("UPDATE Review SET Title = @title, Comment = @comment, Rating = @rating, UpdatedBy = @updatedby, DateUpdated = @timeupdated WHERE Id = @id;", theConnection);

                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@title", review.Title);
                            cmd.Parameters.AddWithValue("@comment", review.Comment);
                            cmd.Parameters.AddWithValue("@rating", review.Rating);
                            cmd.Parameters.AddWithValue("@updatedby", review.UpdatedBy = authenticatedUser);
                            cmd.Parameters.AddWithValue("@timeupdated", review.DateUpdated = DateTime.Now);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                return (review, "Review info edited! (ignore null values, it's all good)");
                            }
                            else
                            {
                                return (null, "Did not edit review.");
                            }
                        }
                        else
                        {
                            return (null, "Did not find a CreatedBy.");
                        }                        
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
        //-------------- DELETE REVIEW BY ID ---------------------------
        public async Task<(bool, string)> DeleteReviewAsync(Guid id)
        {
            string authenticatedUser = System.Web.HttpContext.Current.User.Identity.Name; // dohvaca username za mapiranje UpdatedBy

            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    //---- dohvati rolu --------
                    string role = null;
                    ClaimsPrincipal principal = System.Web.HttpContext.Current.User as ClaimsPrincipal;
                    if (principal != null)
                    {
                        Claim roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                        if (roleClaim != null)
                        {
                            role = roleClaim.Value;
                        }
                    }
                    //-----------------------------

                    SqlCommand cmdRead = new SqlCommand("SELECT * FROM Review WHERE Id = @id", theConnection);
                    cmdRead.Parameters.AddWithValue("@id", id);
                    theConnection.Open();

                    SqlDataReader reader = await cmdRead.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Close();

                        SqlCommand cmdCheck = new SqlCommand("SELECT CreatedBy FROM Review WHERE Id = @id", theConnection);
                        cmdCheck.Parameters.AddWithValue("@id", id);

                        object result = await cmdCheck.ExecuteScalarAsync();
                        if (result != null)
                        {
                            string createdBy = result.ToString();

                            if ((createdBy == authenticatedUser) || (role == "Administrator"))
                            {
                                SqlCommand cmd = new SqlCommand("DELETE FROM Review WHERE Id = @id;", theConnection);
                                cmd.Parameters.AddWithValue("@id", id);

                                int affectedRows = await cmd.ExecuteNonQueryAsync();
                                if (affectedRows > 0)
                                {
                                    return (true, "The review has been deleted!");
                                }
                                else
                                {
                                    return (false, "Failed to delete this review.");
                                }
                            }
                            return (false, "Unauthorized: Only the creator of the review or an administrator can delete it.");
                        }
                        return (false, "Could not find a CreatedBy.");
                    }
                    else
                    {
                        return (false, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (false, "Exception");
            }
        }
        //-------------- GET ALL REVIEWS WITH PAGING, SORTING, FILTERING ---------------------------
        public async Task<(List<Review>, string)> GetAllWithParamsAsync(SearchParam search, SortParam sort, PageParam page)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    StringBuilder sb = new StringBuilder();
                    SqlCommand cmd = new SqlCommand();

                    if (search != null)
                    {
                        sb.Append("SELECT * FROM Review WHERE 1=1");

                        //------------ FILTERING -------------------
                        if (!string.IsNullOrWhiteSpace(search.Title))
                        {
                            sb.Append(" AND Title LIKE @title");
                            cmd.Parameters.AddWithValue("@title", "%" + search.Title + "%");
                        }
                        if (!string.IsNullOrWhiteSpace(search.Comment))
                        {
                            sb.Append(" AND Comment LIKE @comment");
                            cmd.Parameters.AddWithValue("@comment", "%" + search.Comment + "%");
                        }
                        if (search.Rating != 0) // drukcije zbog int
                        {
                            sb.Append(" AND Rating = @rating");
                            cmd.Parameters.AddWithValue("@rating", search.Rating);
                        }
                        if (!string.IsNullOrWhiteSpace(search.CreatedBy))
                        {
                            sb.Append(" AND CreatedBy LIKE @CreatedBy");
                            cmd.Parameters.AddWithValue("@CreatedBy", search.CreatedBy);
                        }
                        if (!string.IsNullOrWhiteSpace(search.UpdatedBy))
                        {
                            sb.Append(" AND UpdatedBy LIKE @UpdatedBy");
                            cmd.Parameters.AddWithValue("@UpdatedBy", search.UpdatedBy);
                        }
                        if (search.CreatedAfter != null && search.CreatedBefore != null)
                        {
                            sb.Append(" AND DateCreated >= @createdafter AND DateCreated <= @createdbefore");
                            cmd.Parameters.AddWithValue("@createdafter", search.CreatedAfter);
                            cmd.Parameters.AddWithValue("@createdbefore", search.CreatedBefore);
                        }
                        else if (search.CreatedAfter != null)
                        {
                            sb.Append(" AND DateCreated >= @createdafter");
                            cmd.Parameters.AddWithValue("@createdafter", search.CreatedAfter);
                        }
                        else if (search.CreatedBefore != null)
                        {
                            sb.Append(" AND DateCreated <= @createdbefore");
                            cmd.Parameters.AddWithValue("@createdbefore", search.CreatedBefore);
                        }
                        if (search.UpdatedAfter != null && search.UpdatedBefore != null)
                        {
                            sb.Append(" AND DateUpdated >= @updatedafter AND DateUpdated <= @updatedbefore");
                            cmd.Parameters.AddWithValue("@updatedafter", search.UpdatedAfter);
                            cmd.Parameters.AddWithValue("@updatedbefore", search.UpdatedBefore);
                        }
                        else if (search.UpdatedAfter != null)
                        {
                            sb.Append(" AND DateUpdated >= @updatedafter");
                            cmd.Parameters.AddWithValue("@updatedafter", search.UpdatedAfter);
                        }
                        else if (search.UpdatedBefore != null)
                        {
                            sb.Append(" AND DateUpdated <= @updatedbefore");
                            cmd.Parameters.AddWithValue("@updatedbefore", search.UpdatedBefore);
                        }

                        //---------------- SORTING -----------------
                        if (sort.OrderByWhat != null && sort.SortDirection != null)
                        {
                            sb.Append($" ORDER BY {sort.OrderByWhat} {sort.SortDirection}");
                        }
                        else if (sort.OrderByWhat != null)
                        {
                            sb.Append($" ORDER BY {sort.OrderByWhat} ASC");
                        }
                        else if (sort.SortDirection != null)
                        {
                            sb.Append($" ORDER BY DateCreated {sort.SortDirection}");
                        }

                        //---------------- PAGING ----------------------
                        if (sort.OrderByWhat == null && sort.SortDirection == null)
                        {
                            sb.Append($" ORDER BY DateCreated DESC");
                        }
                        if (page.PageNumber != null && page.EntriesPerPage != null)
                        {
                            sb.Append(" OFFSET @Offset ROWS FETCH NEXT @EntriesPerPage ROWS ONLY;");
                            int? pageOffset = (page.PageNumber - 1) * page.EntriesPerPage;
                            cmd.Parameters.AddWithValue("@Offset", pageOffset);
                            cmd.Parameters.AddWithValue("@EntriesPerPage", page.EntriesPerPage);
                        }
                        else if (page.PageNumber != null)
                        {
                            sb.Append(" OFFSET @Offset ROWS FETCH NEXT 5 ROWS ONLY;");
                            int? pageOffset = (page.PageNumber - 1) * 5;
                            cmd.Parameters.AddWithValue("@Offset", pageOffset);
                        }
                        else if (page.EntriesPerPage != null)
                        {
                            sb.Append(" OFFSET 0 ROWS FETCH NEXT @EntriesPerPage ROWS ONLY;");
                            cmd.Parameters.AddWithValue("@EntriesPerPage", page.EntriesPerPage);
                        }
                        //-----------------------------------------------------------
                    }
                    else
                    {
                        sb.Append("SELECT * FROM Review");
                    };
                    cmd.Connection = theConnection;
                    cmd.CommandText = sb.ToString();

                    theConnection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Review> list = new List<Review>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Review review = new Review();
                            review.Id = reader.GetGuid(0);
                            review.Title = reader.GetString(1);
                            review.Comment = reader.GetString(2);
                            review.Rating = reader.GetInt32(3);
                            review.BoardGameId = reader.GetGuid(4);
                            review.CreatedBy = reader.GetString(5);
                            review.UpdatedBy = reader.GetString(6);
                            review.DateCreated = reader.GetDateTime(7);
                            review.DateUpdated = reader.GetDateTime(8);
                            review.RegisteredUserId = reader.GetGuid(9);

                            list.Add(review);
                        }
                        reader.Close();
                        return (list, "Success");
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
        //-------------- GET ALL REVIEWS FOR A SPECIFIC BOARD GAME ---------------------------
        public async Task<(List<Review>, string)> GetAllReviewsForOneGame(Guid id, SearchParam search, SortParam sort, PageParam page)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    StringBuilder sb = new StringBuilder();
                    SqlCommand cmd = new SqlCommand();

                    if (search != null)
                    {
                        sb.Append("SELECT * FROM Review WHERE BoardGameId = @id");
                        cmd.Parameters.AddWithValue("@id", id);

                        //------------ FILTERING -------------------
                        if (!string.IsNullOrWhiteSpace(search.Title))
                        {
                            sb.Append(" AND Title LIKE @title");
                            cmd.Parameters.AddWithValue("@title", "%" + search.Title + "%");
                        }
                        if (!string.IsNullOrWhiteSpace(search.Comment))
                        {
                            sb.Append(" AND Comment LIKE @comment");
                            cmd.Parameters.AddWithValue("@comment", "%" + search.Comment + "%");
                        }
                        if (search.Rating != 0) // drukcije zbog int
                        {
                            sb.Append(" AND Rating = @rating");
                            cmd.Parameters.AddWithValue("@rating", search.Rating);
                        }
                        if (!string.IsNullOrWhiteSpace(search.CreatedBy))
                        {
                            sb.Append(" AND CreatedBy LIKE @CreatedBy");
                            cmd.Parameters.AddWithValue("@CreatedBy", search.CreatedBy);
                        }
                        if (!string.IsNullOrWhiteSpace(search.UpdatedBy))
                        {
                            sb.Append(" AND UpdatedBy LIKE @UpdatedBy");
                            cmd.Parameters.AddWithValue("@UpdatedBy", search.UpdatedBy);
                        }
                        if (search.CreatedAfter != null && search.CreatedBefore != null)
                        {
                            sb.Append(" AND DateCreated >= @createdafter AND DateCreated <= @createdbefore");
                            cmd.Parameters.AddWithValue("@createdafter", search.CreatedAfter);
                            cmd.Parameters.AddWithValue("@createdbefore", search.CreatedBefore);
                        }
                        else if (search.CreatedAfter != null)
                        {
                            sb.Append(" AND DateCreated >= @createdafter");
                            cmd.Parameters.AddWithValue("@createdafter", search.CreatedAfter);
                        }
                        else if (search.CreatedBefore != null)
                        {
                            sb.Append(" AND DateCreated <= @createdbefore");
                            cmd.Parameters.AddWithValue("@createdbefore", search.CreatedBefore);
                        }
                        if (search.UpdatedAfter != null && search.UpdatedBefore != null)
                        {
                            sb.Append(" AND DateUpdated >= @updatedafter AND DateUpdated <= @updatedbefore");
                            cmd.Parameters.AddWithValue("@updatedafter", search.UpdatedAfter);
                            cmd.Parameters.AddWithValue("@updatedbefore", search.UpdatedBefore);
                        }
                        else if (search.UpdatedAfter != null)
                        {
                            sb.Append(" AND DateUpdated >= @updatedafter");
                            cmd.Parameters.AddWithValue("@updatedafter", search.UpdatedAfter);
                        }
                        else if (search.UpdatedBefore != null)
                        {
                            sb.Append(" AND DateUpdated <= @updatedbefore");
                            cmd.Parameters.AddWithValue("@updatedbefore", search.UpdatedBefore);
                        }

                        //---------------- SORTING -----------------
                        if (sort.OrderByWhat != null && sort.SortDirection != null)
                        {
                            sb.Append($" ORDER BY {sort.OrderByWhat} {sort.SortDirection}");
                        }
                        else if (sort.OrderByWhat != null)
                        {
                            sb.Append($" ORDER BY {sort.OrderByWhat} ASC");
                        }
                        else if (sort.SortDirection != null)
                        {
                            sb.Append($" ORDER BY DateCreated {sort.SortDirection}");
                        }

                        //---------------- PAGING ----------------------
                        if (sort.OrderByWhat == null && sort.SortDirection == null)
                        {
                            sb.Append($" ORDER BY DateCreated DESC");
                        }
                        if (page.PageNumber != null && page.EntriesPerPage != null)
                        {
                            sb.Append(" OFFSET @Offset ROWS FETCH NEXT @EntriesPerPage ROWS ONLY;");
                            int? pageOffset = (page.PageNumber - 1) * page.EntriesPerPage;
                            cmd.Parameters.AddWithValue("@Offset", pageOffset);
                            cmd.Parameters.AddWithValue("@EntriesPerPage", page.EntriesPerPage);
                        }
                        else if (page.PageNumber != null)
                        {
                            sb.Append(" OFFSET @Offset ROWS FETCH NEXT 5 ROWS ONLY;");
                            int? pageOffset = (page.PageNumber - 1) * 5;
                            cmd.Parameters.AddWithValue("@Offset", pageOffset);
                        }
                        else if (page.EntriesPerPage != null)
                        {
                            sb.Append(" OFFSET 0 ROWS FETCH NEXT @EntriesPerPage ROWS ONLY;");
                            cmd.Parameters.AddWithValue("@EntriesPerPage", page.EntriesPerPage);
                        }
                        //-----------------------------------------------------------
                    }
                    else
                    {
                        sb.Append("SELECT * FROM Review WHERE BoardGameId = @id");
                        cmd.Parameters.AddWithValue("@id", id);
                    };
                    cmd.Connection = theConnection;
                    cmd.CommandText = sb.ToString();

                    theConnection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Review> list = new List<Review>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Review review = new Review();
                            review.Id = reader.GetGuid(0);
                            review.Title = reader.GetString(1);
                            review.Comment = reader.GetString(2);
                            review.Rating = reader.GetInt32(3);
                            review.BoardGameId = reader.GetGuid(4);
                            review.CreatedBy = reader.GetString(5);
                            review.UpdatedBy = reader.GetString(6);
                            review.DateCreated = reader.GetDateTime(7);
                            review.DateUpdated = reader.GetDateTime(8);
                            review.RegisteredUserId = reader.GetGuid(9);

                            list.Add(review);
                        }
                        reader.Close();
                        return (list, "Success");
                    }
                    else
                    {
                        return (null, "No rows found.");
                    }
                }
            }
            catch (Exception)
            {
                return (null, "Exception");
            }
        }
    }
}
