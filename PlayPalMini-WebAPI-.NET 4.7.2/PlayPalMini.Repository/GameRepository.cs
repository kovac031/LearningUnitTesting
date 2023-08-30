using PlayPalMini.Common;
using PlayPalMini.Model;
using PlayPalMini.Repository.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// komentirano sta i kako na UserRepository
namespace PlayPalMini.Repository
{
    public class GameRepository : IGameRepository
    {
        public static string connectionString = "Data Source=VREMENSKISTROJ;Initial Catalog=PlayPalMini;Integrated Security=True";

        //-------------- GET ALL BOARD GAMES ---------------------------
        public async Task<(List<BoardGame>, string)> GetAllAsync()
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    //SqlCommand cmd = new SqlCommand("SELECT * FROM BoardGame", theConnection); // prije nego sam dodao AverageRating

                    string sqlQuery = @"SELECT game.Id, game.Title, CAST(game.Description AS NVARCHAR(MAX)) AS Description, game.CreatedBy, game.UpdatedBy, game.DateCreated, game.DateUpdated, ROUND(AVG(CAST(review.Rating AS FLOAT)),2) AS AverageRating 
                                    FROM BoardGame game LEFT JOIN Review review ON game.Id = review.BoardGameId
                                    GROUP BY game.Id, game.Title, CAST(game.Description AS NVARCHAR(MAX)), game.CreatedBy, game.UpdatedBy, game.DateCreated, game.DateUpdated";

                    SqlCommand cmd = new SqlCommand(sqlQuery, theConnection); // dodao za AverageRating
                    theConnection.Open();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<BoardGame> list = new List<BoardGame>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            BoardGame game = new BoardGame();
                            game.Id = reader.GetGuid(0);
                            game.Title = reader.GetString(1);
                            game.Description = reader.GetString(2);
                            game.CreatedBy = reader.GetString(3);
                            game.UpdatedBy = reader.GetString(4);                        
                            game.DateCreated = reader.GetDateTime(5);
                            game.DateUpdated = reader.GetDateTime(6);
                            game.AverageRating = reader.IsDBNull(7) ? (double?)null : Convert.ToDouble(reader.GetValue(7));

                            list.Add(game);
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
        //-------------- GET ONE BOARD GAME BY ID ---------------------------
        public async Task<(BoardGame, string)> GetOneByIdAsync(Guid id)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    //SqlCommand cmd = new SqlCommand("SELECT * FROM BoardGame WHERE Id = @id", theConnection);

                    string sqlQuery = @"SELECT game.Id, game.Title, CAST(game.Description AS NVARCHAR(MAX)) AS Description, game.CreatedBy, game.UpdatedBy, game.DateCreated, game.DateUpdated, ROUND(AVG(CAST(review.Rating AS FLOAT)),2) AS AverageRating 
                                    FROM BoardGame game LEFT JOIN Review review ON game.Id = review.BoardGameId
                                    WHERE game.Id = @id
                                    GROUP BY game.Id, game.Title, CAST(game.Description AS NVARCHAR(MAX)), game.CreatedBy, game.UpdatedBy, game.DateCreated, game.DateUpdated";

                    SqlCommand cmd = new SqlCommand(sqlQuery, theConnection);

                    cmd.Parameters.AddWithValue("@id", id); 
                    theConnection.Open();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        BoardGame game = new BoardGame(); 

                        while (reader.Read())
                        {
                            game.Id = reader.GetGuid(0);
                            game.Title = reader.GetString(1);
                            game.Description = reader.GetString(2);
                            game.CreatedBy = reader.GetString(3);
                            game.UpdatedBy = reader.GetString(4);
                            game.DateCreated = reader.GetDateTime(5);
                            game.DateUpdated = reader.GetDateTime(6);
                            game.AverageRating = reader.IsDBNull(7) ? (double?)null : Convert.ToDouble(reader.GetValue(7));

                        }
                        reader.Close();
                        return (game, "Success");
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
        //------------------ CREATE BOARD GAME ---------------------
        public async Task<(bool, string)> CreateGameAsync(BoardGame game)
        {
            string authenticatedUser = System.Web.HttpContext.Current.User.Identity.Name;

            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO BoardGame VALUES (@id, @title, @description, @createdby, @updatedby, @timecreated, @timeupdated);", theConnection);

                    cmd.Parameters.AddWithValue("@id", game.Id = Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@title", game.Title);
                    cmd.Parameters.AddWithValue("@description", game.Description);
                    cmd.Parameters.AddWithValue("@createdby", game.CreatedBy = authenticatedUser); // dovoljno samo pokupiti ime, jer je u kontroleru ograniceno na samo Administrator vec
                    cmd.Parameters.AddWithValue("@updatedby", game.UpdatedBy = "n/a");
                    cmd.Parameters.AddWithValue("@timecreated", game.DateCreated = DateTime.Now);
                    cmd.Parameters.AddWithValue("@timeupdated", game.DateUpdated = DateTime.Now);

                    theConnection.Open();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return (true, "Added this new board game!");
                    }
                    else
                    {
                        return (false, "Failed to create new board game.");
                    }
                }
            }
            catch (Exception)
            {
                return (false, "Exception");
            }
        }
        //------------------ EDIT BOARD GAME ---------------------
        public async Task<(BoardGame, string)> EditGameAsync(BoardGame game, Guid id)
        {
            string authenticatedUser = System.Web.HttpContext.Current.User.Identity.Name;

            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmdGet = new SqlCommand("SELECT * FROM BoardGame WHERE Id = @id", theConnection);
                    cmdGet.Parameters.AddWithValue("@id", id);
                    theConnection.Open();

                    SqlDataReader reader = await cmdGet.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Close(); 

                        SqlCommand cmd = new SqlCommand("UPDATE BoardGame SET Title = @title, Description = @description, UpdatedBy = @updatedby, DateUpdated = @timeupdated WHERE Id = @id;", theConnection);

                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@title", game.Title);
                        cmd.Parameters.AddWithValue("@description", game.Description);
                        cmd.Parameters.AddWithValue("@updatedby", game.UpdatedBy = authenticatedUser);
                        cmd.Parameters.AddWithValue("@timeupdated", game.DateUpdated = DateTime.Now);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            return (game, "Game info edited! (ignore null values, it's all good)");
                        }
                        else
                        {
                            return (null, "Did not edit board game.");
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
        //-------------- DELETE BOARD GAME BY ID ---------------------------
        public async Task<(bool, string)> DeleteGameAsync(Guid id)
        {
            try
            {
                SqlConnection theConnection = new SqlConnection(connectionString);
                using (theConnection)
                {
                    SqlCommand cmdRead = new SqlCommand("SELECT * FROM BoardGame WHERE Id = @id", theConnection);
                    cmdRead.Parameters.AddWithValue("@id", id);
                    theConnection.Open();

                    SqlDataReader reader = await cmdRead.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Close();

                        SqlCommand cmd = new SqlCommand("DELETE FROM BoardGame WHERE Id = @id;", theConnection);
                        cmd.Parameters.AddWithValue("@id", id);

                        int affectedRows = await cmd.ExecuteNonQueryAsync(); 
                        if (affectedRows > 0)
                        {
                            return (true, "The game has been deleted!");
                        }
                        else
                        {
                            return (false, "Failed to delete this game.");
                        }
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
        //-------------- GET ALL BOARD GAMES WITH PAGING, SORTING, FILTERING ---------------------------
        public async Task<(List<BoardGame>, string)> GetAllWithParamsAsync(SearchParam search, SortParam sort, PageParam page)
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
                        sb.Append("SELECT game.Id, game.Title, CAST(game.Description AS NVARCHAR(MAX)) AS Description, game.CreatedBy, game.UpdatedBy, game.DateCreated, game.DateUpdated, ROUND(AVG(CAST(review.Rating AS FLOAT)),2) AS AverageRating");
                        sb.Append(" FROM BoardGame game LEFT JOIN Review review ON game.Id = review.BoardGameId WHERE 1=1"); 

                        //------------ FILTERING -------------------
                        if (!string.IsNullOrWhiteSpace(search.Title))
                        {
                            sb.Append(" AND game.Title LIKE @title");
                            cmd.Parameters.AddWithValue("@title", search.Title);
                        }
                        if (!string.IsNullOrWhiteSpace(search.Description))
                        {
                            sb.Append(" AND game.Description LIKE @description");
                            cmd.Parameters.AddWithValue("@description", "%" + search.Description + "%");
                        }
                        if (!string.IsNullOrWhiteSpace(search.CreatedBy))
                        {
                            sb.Append(" AND game.CreatedBy LIKE @CreatedBy");
                            cmd.Parameters.AddWithValue("@CreatedBy", search.CreatedBy);
                        }
                        if (!string.IsNullOrWhiteSpace(search.UpdatedBy))
                        {
                            sb.Append(" AND game.UpdatedBy LIKE @UpdatedBy");
                            cmd.Parameters.AddWithValue("@UpdatedBy", search.UpdatedBy);
                        }
                        if (search.CreatedAfter != null && search.CreatedBefore != null) 
                        {
                            sb.Append(" AND game.DateCreated >= @createdafter AND game.DateCreated <= @createdbefore");
                            cmd.Parameters.AddWithValue("@createdafter", search.CreatedAfter);
                            cmd.Parameters.AddWithValue("@createdbefore", search.CreatedBefore);
                        }
                        else if (search.CreatedAfter != null)
                        {
                            sb.Append(" AND game.DateCreated >= @createdafter"); 
                            cmd.Parameters.AddWithValue("@createdafter", search.CreatedAfter);
                        }
                        else if (search.CreatedBefore != null)
                        {
                            sb.Append(" AND game.DateCreated <= @createdbefore"); 
                            cmd.Parameters.AddWithValue("@createdbefore", search.CreatedBefore);
                        }
                        if (search.UpdatedAfter != null && search.UpdatedBefore != null)
                        {
                            sb.Append(" AND game.DateUpdated >= @updatedafter AND game.DateUpdated <= @updatedbefore");
                            cmd.Parameters.AddWithValue("@updatedafter", search.UpdatedAfter);
                            cmd.Parameters.AddWithValue("@updatedbefore", search.UpdatedBefore);
                        }
                        else if (search.UpdatedAfter != null)
                        {
                            sb.Append(" AND game.DateUpdated >= @updatedafter"); 
                            cmd.Parameters.AddWithValue("@updatedafter", search.UpdatedAfter);
                        }
                        else if (search.UpdatedBefore != null)
                        {
                            sb.Append(" AND game.DateUpdated <= @updatedbefore"); 
                            cmd.Parameters.AddWithValue("@updatedbefore", search.UpdatedBefore);
                        }
                        //-----------------------------------------
                        sb.Append(@" GROUP BY game.Id, game.Title, CAST(game.Description AS NVARCHAR(MAX)), game.CreatedBy, game.UpdatedBy, game.DateCreated, game.DateUpdated");

                        //--------AVERAGE RATING JE GLUP----------- // pošto nije u BoardGame tablici, ne može sa WHERE, mora sa HAVING koji ide nakon GROUP BY
                        if (search.AverageGreaterThan != null && search.AverageLessThan != null)
                        {
                            sb.Append(" HAVING ROUND(AVG(CAST(review.Rating AS FLOAT)),2) >= @AverageGreaterThan AND ROUND(AVG(CAST(review.Rating AS FLOAT)),2) <= @AverageLessThan");
                            cmd.Parameters.AddWithValue("@AverageGreaterThan", search.AverageGreaterThan);
                            cmd.Parameters.AddWithValue("@AverageLessThan", search.AverageLessThan);
                        }
                        else if (search.AverageGreaterThan != null)
                        {
                            sb.Append(" HAVING ROUND(AVG(CAST(review.Rating AS FLOAT)),2) >= @AverageGreaterThan");
                            cmd.Parameters.AddWithValue("@AverageGreaterThan", search.AverageGreaterThan);
                        }
                        else if (search.AverageLessThan != null)
                        {
                            sb.Append(" HAVING ROUND(AVG(CAST(review.Rating AS FLOAT)),2) <= @AverageLessThan");
                            cmd.Parameters.AddWithValue("@AverageLessThan", search.AverageLessThan);
                        }
                        //---------------- SORTING ----------------
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
                            sb.Append($" ORDER BY game.DateCreated {sort.SortDirection}");
                        }

                        //---------------- PAGING ----------------------
                        if (sort.OrderByWhat == null && sort.SortDirection == null) 
                        {
                            sb.Append($" ORDER BY game.DateCreated DESC");
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
                        sb.Append("SELECT game.Id, game.Title, CAST(game.Description AS NVARCHAR(MAX)) AS Description, game.CreatedBy, game.UpdatedBy, game.DateCreated, game.DateUpdated, ROUND(AVG(CAST(review.Rating AS FLOAT)),2) AS AverageRating");
                        sb.Append(" FROM BoardGame game LEFT JOIN Review review ON game.Id = review.BoardGameId GROUP BY game.Id, game.Title, CAST(game.Description AS NVARCHAR(MAX)), game.CreatedBy, game.UpdatedBy, game.DateCreated, game.DateUpdated"); 
                    };
                    cmd.Connection = theConnection;
                    cmd.CommandText = sb.ToString(); 

                    theConnection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<BoardGame> list = new List<BoardGame>();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            BoardGame game = new BoardGame();
                            game.Id = reader.GetGuid(0);
                            game.Title = reader.GetString(1);
                            game.Description = reader.GetString(2);
                            game.CreatedBy = reader.GetString(3);
                            game.UpdatedBy = reader.GetString(4);
                            game.DateCreated = reader.GetDateTime(5);
                            game.DateUpdated = reader.GetDateTime(6);
                            game.AverageRating = reader.IsDBNull(7) ? (double?)null : Convert.ToDouble(reader.GetValue(7));

                            list.Add(game);
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
