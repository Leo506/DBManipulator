using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace DBTest
{
    class DBController
    {
        private const string connectionString = "server=127.0.0.1;uid=root;pwd=26041986;database=worldskills";
        private MySqlConnection currentConnection;


        /// <summary>
        /// Создаёт новое соединение с базой данных
        /// </summary>
        public DBController()
        {
            currentConnection = new MySqlConnection(connectionString);
            currentConnection.Open();
            Console.WriteLine($"Connection with db was opened {currentConnection.State == System.Data.ConnectionState.Open}");
        }


        ~DBController()
        {
            currentConnection.Close();
            currentConnection.Dispose();
            Console.WriteLine($"Connection wiht db was closed");
        }


        /// <summary>
        /// Возвращает список всех игроков
        /// </summary>
        /// <returns>Список игроков, представленных в структуре Player</returns>
        public List<Player> GetAllPlayers()
        {
            List<Player> players = new List<Player>();
            MySqlDataReader reader = ExecuteCommandWithReader("SELECT * FROM PlayerInfo;");
            while(reader.Read())
            {
                // 0 - id
                // 1 - email
                // 2 - password
                // 3 - last online
                // 4 - registration data

                int id = int.Parse(reader[0].ToString());
                DateTime latOnline = DateTime.Parse(reader[3].ToString());
                DateTime registration = DateTime.Parse(reader[4].ToString());
                players.Add(new Player(id, reader[1].ToString(), reader[2].ToString(), latOnline, registration));
            }
            reader.Close();
            reader.Dispose();

            return players;
        }


        /// <summary>
        /// Возвращает список всех настроект всех  игроков
        /// </summary>
        /// <returns>Список настроек, представленный в виде структуры PlayerSettings</returns>
        public List<PlayerSettings> GetAllPlayersSettings()
        {
            List<PlayerSettings> settings = new List<PlayerSettings>();
            MySqlDataReader reader = ExecuteCommandWithReader("SELECT * FROM PlayerSettings;");
            while (reader.Read())
            {
                int id = int.Parse(reader[0].ToString());
                bool sound = int.Parse(reader[1].ToString()) == 1 ? true : false;
                bool music = int.Parse(reader[2].ToString()) == 1 ? true : false;
                settings.Add(new PlayerSettings(id, sound, music, reader[3].ToString()));
            }
            reader.Close();
            reader.Dispose();

            return settings;
        }


        /// <summary>
        /// Возвращает настройки конкретного игрока
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <returns></returns>
        public PlayerSettings GetPlayerSettings(int playerId)
        {
            MySqlDataReader reader = ExecuteCommandWithReader($"SELECT * FROM PlayerSettings WHERE id_player={playerId};");
            PlayerSettings settings = new PlayerSettings();
            while(reader.Read())
            {
                int id = int.Parse(reader[0].ToString());
                bool sound = int.Parse(reader[1].ToString()) == 1 ? true : false;
                bool music = int.Parse(reader[2].ToString()) == 1 ? true : false;
                settings = new PlayerSettings(id, sound, music, reader[3].ToString());
            }
            reader.Close();
            reader.Dispose();
            return settings;
        }


        /// <summary>
        /// Добавляет нового игрока в базу
        /// </summary>
        /// <param name="player">Структура, представляющая данные об игроке</param>
        public void AddNewPlayer(Player player)
        {
            string online = player.lastOnline.ToString("yyyy-MM-dd HH:mm:ss");
            string registrate = player.registrationDate.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "INSERT INTO PlayerInfo (email, password, last_online, registration_date) " +
                $"VALUES (\"{player.email}\", \"{player.password}\", \"{online}\", \"{registrate}\");";
            string sql2 = "INSERT INTO PlayerSettings (id_player) VALUES ((SELECT id_player FROM playerInfo ORDER BY id_player DESC LIMIT 1))";
            ExecuteCommand(sql);
            ExecuteCommand(sql2);
        }


        /// <summary>
        /// Устанавливает настройка для конкрктного игрока
        /// </summary>
        /// <param name="playerID">ID игрока</param>
        /// <param name="settings">Настройки</param>
        public void SetSettingsForPlayer(int playerID, PlayerSettings settings)
        {
            int sound = settings.soundOn ? 1 : 0;
            int music = settings.musicOn ? 1 : 0;
            string sql = $"UPDATE PlayerSettings SET sounds={sound}, music={music}, language=\"{settings.language}\" WHERE id_player={playerID};";
            ExecuteCommand(sql);
        }


        /// <summary>
        /// Добавляет покупку игрока в базу
        /// </summary>
        /// <param name="playerID">ID игрока</param>
        /// <param name="productID">ID товара</param>
        public void AddPlayerPurchace(int playerID, int productID)
        {
            int countOfPurchaces = 0;
            object obj = ExecuteCommandWithScalar($"select count from playerpurchace where id_player={playerID} and id_product={productID};");
            if (obj != null)
                countOfPurchaces = int.Parse(obj.ToString());

            string sql = "";
            if (countOfPurchaces == 0)
                sql = $"insert into playerpurchace (id_player, id_product) values ({playerID}, {productID});";
            else
                sql = $"update playerpurchace set count = {++countOfPurchaces} where id_player={playerID} and id_product={productID};";

            ExecuteCommand(sql);
        }


        /// <summary>
        /// Возвращает кол-во определенного товара, купленного определенным игроком
        /// </summary>
        /// <param name="playerID">ID игрока</param>
        /// <param name="productID">ID товара</param>
        /// <returns></returns>
        public int GetPlayerPurchace(int playerID, int productID)
        {
            int count = 0;
            object obj = ExecuteCommandWithScalar($"select count from playerpurchace where id_player={playerID} and id_product={productID};");
            if (obj != null)
                count = int.Parse(obj.ToString());

            return count;
        }


        /// <summary>
        /// Выполняет SQL команды, где нужно извлекать данные из базы
        /// </summary>
        /// <param name="commandText">SQL команда</param>
        /// <returns>Возвращает экземпляр MySqlDateReader, который нужно закрыть после использования</returns>
        private MySqlDataReader ExecuteCommandWithReader(string commandText)
        {
            MySqlCommand command = new MySqlCommand(commandText, currentConnection);
            return command.ExecuteReader();
        }


        /// <summary>
        /// Выполняет SQL команды по изменению/добавлению данных в базе
        /// </summary>
        /// <param name="commandText">SQL команда</param>
        private void ExecuteCommand(string commandText)
        {
            MySqlCommand command = new MySqlCommand(commandText, currentConnection);
            command.ExecuteNonQuery();
        }


        /// <summary>
        /// Выполняет SQL команду возвращающее единственный объект из базы
        /// </summary>
        /// <param name="commandText">SQL команда</param>
        /// <returns></returns>
        private object ExecuteCommandWithScalar(string commandText)
        {
            MySqlCommand command = new MySqlCommand(commandText, currentConnection);
            return command.ExecuteScalar();
        }
    }
}
