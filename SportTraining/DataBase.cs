using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;//вставка
using System.Data.Common;//получение
using System.Drawing;
using System.IO;

namespace SportTraining
{
    class DataBase
    {

        //вставка данных
        public void setData(string path, Sportsmen sportsmen, Workout workout)
        {
            /***************************************************** 
            * На диске C:\ находится база данных Microsoft Access 2007
            * В базе данных одна таблица. Название таблицы: tab.
            * Поля:
            * id - Счетчик
            * image - Поле объекта OLE
            * В папке `Мои рисунки` находится файл Alien 1.bmp, который будем вставлять в БД.
            *******************************************************/
            //OleDbConnection oleDbConnection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\db.accdb");  //Подключение к БД.
            OleDbConnection oleDbConnection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path);     
            //OleDbCommand oleDbCommand = new OleDbCommand("INSERT INTO `Tab` ( `image` ) VALUES ( ? ) ", oleDbConnection);         //Запрос на вставку
            OleDbCommand oleDbCommand = new OleDbCommand("INSERT INTO `workandresult` ( `work`, 'numWork', 'code' ) VALUES ( " + workout.work + ", " + workout.number + ", " + sportsmen.id + ")", oleDbConnection);
            oleDbCommand = new OleDbCommand("INSERT INTO `workandresult` ( `work` ) VALUES ( " + workout.work + " ) WHEN ('code' = " + sportsmen.id + ")", oleDbConnection);        
            
            OleDbParameter oleDbParameter = new OleDbParameter("image", OleDbType.VarBinary);                                     //Параметр запроса
            string fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\Alien 1.bmp";                  //Путь к файлу
            Image image = Image.FromFile(fileName);                                                                               //Изображение из файла.
            MemoryStream memoryStream = new MemoryStream();                                                                       //Поток в который запишем изображение
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);                                                     //Сохраняем изображение в поток.
            oleDbParameter.Value = memoryStream.ToArray();                                                                        //Устанавливаем значение параметра
            oleDbCommand.Parameters.Add(oleDbParameter);                                                                          //Добавляем параметр
            oleDbConnection.Open();                                                                                               //Открываем соединение с БД
            oleDbCommand.ExecuteNonQuery();                                                                                       //Выполняем запрос.
            oleDbConnection.Close();                                                                                              //Закрываем соединение
            memoryStream.Dispose();     
        }
        //получение данных
        public void getData(string path)
        { 
            /***************************************************** 
             * На диске C:\ находится база данных Microsoft Access 2007
             * В базе данных одна таблица. Название таблицы: tab.
             * Поля:
             * id - Счетчик
             * image - Поле объекта OLE
             * В папке `Мои рисунки` находится файл Alien 1.bmp, который будем вставлять в БД.
             *******************************************************/
            OleDbConnection oleDbConnection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\db.accdb"); //Создаем подключение
            OleDbCommand oleDbCommand = new OleDbCommand("SELECT image FROM Tab WHERE id = 1", oleDbConnection);                 //Запрос на выборку
            oleDbConnection.Open();                                                                                              //Открываем соединение   
            OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();                                                      //Делаем выборку

            if (oleDbDataReader.HasRows)                                                                                         //Проверяем есть ли в выборке строки
            {
                MemoryStream memoryStream = new MemoryStream();                                                                  //Создаем поток, в котором будем хранить изображение         
                foreach (DbDataRecord record in oleDbDataReader)                                                                 //Цикл для всех записей, полученных в результате выборки
                    memoryStream.Write((byte[])record["image"], 0, ((byte[])record["image"]).Length);                            //Пишем в поток
                Image image = Image.FromStream(memoryStream);                                                                    //Получаем изображение из потока
                image.Save(@"C:\alien.BMP");                                                                                     //Сохраняем изображение на диск C:\
                memoryStream.Dispose();                                                                                          //Освобождаем память
            }
            else
                Console.Write("Запрос вернул ноль строк");                                                                       //Вывод сообщения
            oleDbConnection.Close();
        }
    }
}
