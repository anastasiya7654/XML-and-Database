using System;
using System.Xml;// для работы с XML
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace ConsoleApp2
{
    class Program
    {
        static string connectionString = @"Data Source=LAPTOP-RCGMJU2P\MSSQLSERVER01;Initial Catalog=Magazin;Integrated Security=True";
        SqlConnection connection = new SqlConnection (@"Data Source=LAPTOP-RCGMJU2P\MSSQLSERVER01;Initial Catalog=Magazin;Integrated Security=True");
        //метод для заполнения данных из xml документа в базу данных
        //в метод передаем переменные, которыми будем заполнять таблицу
        public static void InsertData (int id, string type, int bid, int cbid, bool available, string url,
            int price, string currencyId, int categoryId, string categorytype, string picture,
            bool delivery, int local_delivery_cost, string typePrefix, string vendor, string vendorCode,
            string model, bool manufacturer_warranty, string country_of_origin, string description)
        {
                                   //название хранимой процедуры
            string SqlExpression = "InsertData";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                /* создается экземпляр класса SqlCommand
                ему присваивается тип - хранимая процедура и
                передаются соответствующие параметры, также есть
                в коде процедуры на sql server
                */
                SqlCommand command = new SqlCommand(SqlExpression, connection);           
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter parameter;
                parameter = command.Parameters.AddWithValue("@id", id);
                parameter = command.Parameters.AddWithValue("@type", type);
                parameter = command.Parameters.AddWithValue("@bid", bid);
                parameter = command.Parameters.AddWithValue("@cbid", cbid);
                parameter = command.Parameters.AddWithValue("@available", available);
                parameter = command.Parameters.AddWithValue("@url", url);
                parameter = command.Parameters.AddWithValue("@price", price);
                parameter = command.Parameters.AddWithValue("@currencyId", currencyId);
                parameter = command.Parameters.AddWithValue("@categoryId", categoryId);
                parameter = command.Parameters.AddWithValue("@categorytype", categorytype);
                parameter = command.Parameters.AddWithValue("@picture", picture);
                parameter = command.Parameters.AddWithValue("@delivery", delivery);
                parameter = command.Parameters.AddWithValue("@local_delivery_cost", local_delivery_cost);
                parameter = command.Parameters.AddWithValue("@typePrefix", typePrefix);
                parameter = command.Parameters.AddWithValue("@vendor", vendor);
                parameter = command.Parameters.AddWithValue("@vendorCode", vendorCode);
                parameter = command.Parameters.AddWithValue("@model", model);
                parameter = command.Parameters.AddWithValue("@manufacturer_warranty", manufacturer_warranty);
                parameter = command.Parameters.AddWithValue("@country_of_origin", country_of_origin);
                parameter = command.Parameters.AddWithValue("@description", description);           
         
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Операция выполнена. Теперь можно увидеть изменения, открыв таблицу на сервере");
                }
                catch (Exception)
                {
                    Console.WriteLine("Произошла ошибка. Возможно, вы попытались вставить уже существующее значение.");
                    throw;
                }
            }
        }        
     
        static void Main(string[] args)
        {
            Console.WriteLine
                ("Здравствуйте! Эта программа создает массив объектов и заполняет его в" +
                "\nсоответствии с данными из magazin.xml. Также в методе IndertData() происходит " +
                "\nавтозаполнение таблицы Offer1 в базе данных Magazin, которая соответствует объекту с" +
                "\nтипом vendormodel в xml документе. В эту таблицу добавлено поле SysId для удобства " +
                "\nтестирования программы и избежания ошибок, касающихся повторного внесения primary key. В" +
                "\nостальном поля таблицы полностью соответствуют объекту." +
                "\nБуду рада комментариям по поводу оптимизации кода!");
            // создание массива объектов offer
            Offer[] offers = new Offer[7];
            // загрузка XML из файла
            var document = new XmlDocument();
            document.Load("magazin.xml");
            //вывод
            //получим корневой элемент
            XmlElement xRoot = document.DocumentElement;
           //обходим дочерние элементы в корневом элементе
            foreach (XmlNode childnode in xRoot )
            {
                // если узел - shop
                if (childnode.Name=="shop")
                {                   
                    // обходим все дочерние элементы в элементе shop
                    foreach (XmlNode childestnode in childnode)
                    {
                        // если узел - offers
                        if (childestnode.Name == "offers")
                        {
                            // Console.WriteLine($"ПРОВЕРКА - Я нашел offers");
                            // выберем узлы offer
                            XmlNodeList nodes = childestnode.SelectNodes("offer");
                            int i = - 1;
                            //для каждого узла в offer
                            foreach (XmlNode n in nodes)
                            {
                                                              
                                switch (n.SelectSingleNode("@type").Value)
                                {
                                    case "vendor.model":
                                        //счетчик количества атрибутов в объекте
                                        Vendormodel tovar = new Vendormodel();
                                        tovar.id = int.Parse(n.SelectSingleNode("@id").Value);
                                        tovar.type = n.SelectSingleNode("@type").Value;
                                        tovar.bid = int.Parse(n.SelectSingleNode("@bid").Value);
                                        tovar.cid = int.Parse(n.SelectSingleNode("@cbid").Value);                                      
                                        
                                        tovar.available = bool.Parse(n.SelectSingleNode("@available").Value);
                                        // для каждого offer
                                        foreach (XmlNode childnode1 in n)
                                        {
                                            // Console.WriteLine("ПРОВЕРКА - Ищу в  vendormodel");
                                            if (childnode1.Name == "url") tovar.url = childnode1.InnerText;
                                            if (childnode1.Name == "price") tovar.price = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "currencyId") tovar.currencyid = childnode1.InnerText;
                                            if (childnode1.Name == "categoryId")
                                            {
                                                tovar.categoryid = int.Parse(childnode1.InnerText);
                                                tovar.categorytype = childnode1.SelectSingleNode("@type").Value;                                                
                                            }                                           
                                            if (childnode1.Name == "picture") tovar.picture = childnode1.InnerText;
                                            if (childnode1.Name == "delivery") tovar.delivery = bool.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "local_delivery_cost") tovar.local_delivery_cost = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "typePrefix") tovar.typePrefix = childnode1.InnerText;
                                            if (childnode1.Name == "vendor") tovar.vendor = childnode1.InnerText;
                                            if (childnode1.Name == "vendorCode") tovar.vendorCode = childnode1.InnerText;
                                            if (childnode1.Name == "model") tovar.model = childnode1.InnerText;
                                            if (childnode1.Name == "description") tovar.description = childnode1.InnerText;
                                            if (childnode1.Name == "manufacturer_warranty") tovar.manufacturer_warranty = bool.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "country_of_origin") tovar.country_of_origin = childnode1.InnerText;
                                        }
                                        i++;
                                        offers[i] = tovar;
                                        Console.WriteLine("\n\nТовар : " + (i+1));
                                        Console.WriteLine("Идентификационный номер : " + tovar.id);
                                        Console.WriteLine("BID : " + tovar.bid);
                                        Console.WriteLine("CBID : " + tovar.cid);
                                        Console.WriteLine("Доступность : " + tovar.available);
                                        Console.WriteLine("URL : " + tovar.url);
                                        Console.WriteLine("Цена : " + tovar.price);
                                        Console.WriteLine("Валюта : " + tovar.currencyid);
                                        Console.WriteLine("ID категории : " + tovar.categoryid);
                                        Console.WriteLine("Тип: " + tovar.categorytype);
                                        Console.WriteLine("Изображение : " + tovar.picture);
                                        Console.WriteLine("Доставка : " + tovar.delivery);
                                        Console.WriteLine("Cтоимость местной доставки : " + tovar.local_delivery_cost);
                                        Console.WriteLine("Название : " + tovar.typePrefix);
                                        Console.WriteLine("Производитель: " + tovar.vendor);
                                        Console.WriteLine("Код производителя: " + tovar.vendorCode);
                                        Console.WriteLine("Модель: " + tovar.model);
                                        Console.WriteLine("Описание : " + tovar.description);
                                        Console.WriteLine("Гарантия производителя: " + tovar.manufacturer_warranty);
                                        Console.WriteLine("Место производства: " + tovar.country_of_origin);

                                        InsertData(tovar.id, tovar.type, tovar.bid, tovar.cid, tovar.available, tovar.url,tovar.price, tovar.currencyid,
                                        tovar.categoryid, tovar.categorytype, tovar.picture, tovar.delivery, tovar.local_delivery_cost, tovar.typePrefix,
                                        tovar.vendor,tovar.vendorCode, tovar.model, tovar.manufacturer_warranty, tovar.country_of_origin,tovar.description);
                                        break;
                                    case "book":
                                        book tovarB = new book ();
                                        tovarB.id = int.Parse(n.SelectSingleNode("@id").Value);
                                        tovarB.type = n.SelectSingleNode("@type").Value;
                                        tovarB.bid = int.Parse(n.SelectSingleNode("@bid").Value);
                                        tovarB.available = bool.Parse(n.SelectSingleNode("@available").Value);
                                        foreach (XmlNode childnode1 in n)
                                        {
                                           // Console.WriteLine("ПРОВЕРКА - Ищу в book");
                                            if (childnode1.Name == "url") tovarB.url = childnode1.InnerText;
                                            if (childnode1.Name == "price") tovarB.price = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "currencyId") tovarB.currencyid = childnode1.InnerText;
                                            if (childnode1.Name == "categoryId")
                                            {
                                                tovarB.categoryid = int.Parse(childnode1.InnerText);
                                                tovarB.categorytype = childnode1.SelectSingleNode("@type").Value;
                                            }
                                            if (childnode1.Name == "picture") tovarB.picture = childnode1.InnerText;
                                            if (childnode1.Name == "delivery") tovarB.delivery = bool.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "local_delivery_cost") tovarB.local_delivery_cost = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "author") tovarB.author = childnode1.InnerText;
                                            if (childnode1.Name == "name") tovarB.name = childnode1.InnerText;
                                            if (childnode1.Name == "publisher") tovarB.publisher = childnode1.InnerText;
                                            if (childnode1.Name == "series") tovarB.series = childnode1.InnerText;
                                            if (childnode1.Name == "description") tovarB.description = childnode1.InnerText;
                                            if (childnode1.Name == "year") tovarB.year = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "ISBN") tovarB.ISBN = childnode1.InnerText;
                                            if (childnode1.Name == "volume") tovarB.volume= int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "part") tovarB.part = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "language") tovarB.language = childnode1.InnerText;
                                            if (childnode1.Name == "binding") tovarB.binding = childnode1.InnerText;
                                            if (childnode1.Name == "downloadable") tovarB.downloadable = bool.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "page_extent") tovarB.page_extent = int.Parse(childnode1.InnerText);
                                        }
                                        i++;
                                        offers[i] = tovarB;                                   
                                        Console.WriteLine("\nТовар : " + (i + 1));
                                        Console.WriteLine("Идентификационный номер : " + tovarB.id);
                                        Console.WriteLine("Тип: " + tovarB.type);                                        
                                        Console.WriteLine("BID : " + tovarB.bid);                                      
                                        Console.WriteLine("Доступность : " + tovarB.available);
                                        Console.WriteLine("URL : " + tovarB.url);
                                        Console.WriteLine("Цена : " + tovarB.price);
                                        Console.WriteLine("Валюта : " + tovarB.currencyid);
                                        Console.WriteLine("ID категории : " + tovarB.categoryid);
                                        Console.WriteLine("Категория : " + tovarB.categorytype);
                                        Console.WriteLine("Изображение : " + tovarB.picture);
                                        Console.WriteLine("Доставка : " + tovarB.delivery);
                                        Console.WriteLine("Cтоимость местной доставки : " + tovarB.local_delivery_cost);
                                        Console.WriteLine("Автор : " + tovarB.author);
                                        Console.WriteLine("Название : " + tovarB.name);
                                        Console.WriteLine("Издательство : " + tovarB.publisher);
                                        Console.WriteLine("Серия : " + tovarB.series);
                                        Console.WriteLine("Год : " + tovarB.year);
                                        Console.WriteLine("ISBN : " + tovarB.ISBN);
                                        Console.WriteLine("Том: " + tovarB.volume);
                                        Console.WriteLine("Часть : " + tovarB.part);
                                        Console.WriteLine("Язык : " + tovarB.language);
                                        Console.WriteLine("Обложка : " + tovarB.binding);
                                        Console.WriteLine("Количество страниц : " + tovarB.page_extent);
                                        Console.WriteLine("Описание : " + tovarB.description);
                                        Console.Write("Доступно для скачивания : " + tovarB.downloadable);                                       
                                        break;
                                    case "audiobook":
                                        audiobook tovarA = new audiobook();
                                        tovarA.id = int.Parse(n.SelectSingleNode("@id").Value);
                                        tovarA.type = n.SelectSingleNode("@type").Value;
                                        tovarA.bid = int.Parse(n.SelectSingleNode("@bid").Value);
                                        tovarA.available = bool.Parse(n.SelectSingleNode("@available").Value);
                                        foreach (XmlNode childnode1 in n)
                                        {
                                            // Console.WriteLine("ПРОВЕРКА - Ищу в audiobook ");
                                            if (childnode1.Name == "url") tovarA.url = childnode1.InnerText;
                                            if (childnode1.Name == "price") tovarA.price = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "currencyId") tovarA.currencyid = childnode1.InnerText;
                                            if (childnode1.Name == "categoryId")
                                            {
                                                tovarA.categoryid = int.Parse(childnode1.InnerText);
                                                tovarA.categorytype = childnode1.SelectSingleNode("@type").Value;
                                            }
                                            if (childnode1.Name == "picture") tovarA.picture = childnode1.InnerText;
                                            if (childnode1.Name == "delivery") tovarA.delivery = bool.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "author") tovarA.author = childnode1.InnerText;
                                            if (childnode1.Name == "name") tovarA.name = childnode1.InnerText;
                                            if (childnode1.Name == "publisher") tovarA.publisher = childnode1.InnerText;
                                            if (childnode1.Name == "description") tovarA.description = childnode1.InnerText;
                                            if (childnode1.Name == "year") tovarA.year = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "ISBN") tovarA.ISBN = childnode1.InnerText;
                                            if (childnode1.Name == "language") tovarA.language = childnode1.InnerText;
                                            if (childnode1.Name == "perfomed_by") tovarA.perfomed_by = childnode1.InnerText;
                                            if (childnode1.Name == "perfomance_type ") tovarA.perfomance_type = childnode1.InnerText;
                                            if (childnode1.Name == "storage") tovarA.storage =childnode1.InnerText;
                                            if (childnode1.Name == "format") tovarA.format = childnode1.InnerText;
                                            if (childnode1.Name == "recording_length") tovarA.recording_length = childnode1.InnerText;
                                            if (childnode1.Name == "downadable") tovarA.downadable = bool.Parse(childnode1.InnerText);
                                        }
                                        i++;
                                        offers[i] = tovarA;
                                        Console.WriteLine("\n\nТовар : " + (i + 1));
                                        Console.WriteLine("Идентификационный номер : " + tovarA.id);
                                        Console.WriteLine("Тип: " + tovarA.type);
                                        Console.WriteLine("BID : " + tovarA.bid);
                                        Console.WriteLine("Доступность : " + tovarA.available);
                                        Console.WriteLine("URL : " + tovarA.url);
                                        Console.WriteLine("Цена : " + tovarA.price);
                                        Console.WriteLine("Валюта : " + tovarA.currencyid);
                                        Console.WriteLine("ID категории : " + tovarA.categoryid);
                                        Console.WriteLine("Категория : " + tovarA.categorytype);
                                        Console.WriteLine("Изображение : " + tovarA.picture);
                                        Console.WriteLine("Автор : " + tovarA.author);
                                        Console.WriteLine("Название : " + tovarA.name);
                                        Console.WriteLine("Издательство : " + tovarA.publisher);
                                        Console.WriteLine("Год : " + tovarA.year);
                                        Console.WriteLine("ISBN : " + tovarA.ISBN);
                                        Console.WriteLine("Язык : " + tovarA.language);
                                        Console.WriteLine("Исполнено : " + tovarA.perfomed_by);
                                        Console.WriteLine("Тип исполнения : " + tovarA.perfomance_type);
                                        Console.WriteLine("Носитель : " + tovarA.storage);
                                        Console.WriteLine("Формат : " + tovarA.format);
                                        Console.WriteLine("Длина записи : " + tovarA.recording_length);
                                        Console.WriteLine("Описание : " + tovarA.description);
                                        Console.Write("Доступно для скачивания : " + tovarA.downadable+"\n");
                                        break;
                                    case "artist.title":
                                        artisttitle tovarT = new artisttitle();
                                        tovarT.id = int.Parse(n.SelectSingleNode("@id").Value);
                                        tovarT.type = n.SelectSingleNode("@type").Value;
                                        tovarT.bid = int.Parse(n.SelectSingleNode("@bid").Value);
                                        tovarT.available = bool.Parse(n.SelectSingleNode("@available").Value);
                                        foreach (XmlNode childnode1 in n)
                                        {
                                            // Console.WriteLine("ПРОВЕРКА - Ищу в artisttitle ");
                                            if (childnode1.Name == "url") tovarT.url = childnode1.InnerText;
                                            if (childnode1.Name == "price") tovarT.price = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "currencyId") tovarT.currencyid = childnode1.InnerText;
                                            if (childnode1.Name == "categoryId")
                                            {
                                                tovarT.categoryid = int.Parse(childnode1.InnerText);
                                                tovarT.categorytype = childnode1.SelectSingleNode("@type").Value;
                                            }
                                            if (childnode1.Name == "picture") tovarT.picture = childnode1.InnerText;
                                            if (childnode1.Name == "delivery") tovarT.delivery = bool.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "artist") tovarT.artist = childnode1.InnerText;
                                            if (childnode1.Name == "starring") tovarT.starring = childnode1.InnerText;
                                            if (childnode1.Name == "director") tovarT.director = childnode1.InnerText;
                                            if (childnode1.Name == "originalName") tovarT.originaName = childnode1.InnerText;
                                            if (childnode1.Name == "country") tovarT.country = childnode1.InnerText;
                                            if (childnode1.Name == "title") tovarT.title = childnode1.InnerText;
                                            if (childnode1.Name == "media") tovarT.media = childnode1.InnerText;
                                            if (childnode1.Name == "description") tovarT.description = childnode1.InnerText;
                                            if (childnode1.Name == "year") tovarT.year = int.Parse(childnode1.InnerText);                                           
                                        }
                                        i++;
                                        offers[i] = tovarT;
                                        Console.WriteLine("\n\nТовар : " + (i + 1));
                                        Console.WriteLine("Идентификационный номер : " + tovarT.id);
                                        Console.WriteLine("Тип: " + tovarT.type);
                                        Console.WriteLine("BID : " + tovarT.bid);
                                        Console.WriteLine("Доступность : " + tovarT.available);
                                        Console.WriteLine("URL : " + tovarT.url);
                                        Console.WriteLine("Цена : " + tovarT.price);
                                        Console.WriteLine("Валюта : " + tovarT.currencyid);
                                        Console.WriteLine("ID категории : " + tovarT.categoryid);
                                        Console.WriteLine("Категория : " + tovarT.categorytype);
                                        Console.WriteLine("Изображение : " + tovarT.picture);
                                        Console.WriteLine("Доставка : " + tovarT.delivery);
                                        Console.WriteLine("Артист : " + tovarT.artist);
                                        Console.WriteLine("Название: " + tovarT.title);
                                        Console.WriteLine("Год : " + tovarT.year);
                                        Console.WriteLine("Формат : " + tovarT.media);
                                        Console.WriteLine("Артисты : " + tovarT.starring);
                                        Console.WriteLine("Режиссер : " + tovarT.director);
                                        Console.WriteLine("Оригинальное название : " + tovarT.originaName);
                                        Console.WriteLine("Страна : " + tovarT.country);                                       
                                        Console.WriteLine("Описание : " + tovarT.description);                                       
                                        break;
                                    case "tour":
                                        tour tovarTour = new tour();
                                        tovarTour.id = int.Parse(n.SelectSingleNode("@id").Value);
                                        tovarTour.type = n.SelectSingleNode("@type").Value;
                                        tovarTour.bid = int.Parse(n.SelectSingleNode("@bid").Value);
                                        tovarTour.available = bool.Parse(n.SelectSingleNode("@available").Value);                                        
                                        XmlNodeList nodeList;
                                        nodeList = n.SelectNodes("dataTour");
                                        // int count = nodeList.Count;
                                        //Console.WriteLine(" НАШЕЛ" + nodeList.Count);
                                        foreach (XmlNode node in nodeList)
                                        {
                                          //  Console.WriteLine("ПРОВЕРКА - Чтение одноименных узлов");
                                            if (node.Name==node.NextSibling.Name)
                                                {
                                                    
                                                    tovarTour.dataTour0 = DateTime.Parse(node.InnerText);
                                                    tovarTour.dataTour1 = DateTime.Parse(node.NextSibling.InnerText);

                                                    //Console.WriteLine("А это даты " + tovarTour.dataTour0 + "  " + tovarTour.dataTour1);
                                                } 
                                                                         
                                        }
                                        foreach (XmlNode childnode1 in n)
                                        {
                                            // Console.WriteLine("Ищу в tovarTour");                                           
                                            if (childnode1.Name == "url") tovarTour.url = childnode1.InnerText;
                                            if (childnode1.Name == "price") tovarTour.price = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "currencyId") tovarTour.currencyid = childnode1.InnerText;
                                            if (childnode1.Name == "categoryId")
                                            {
                                                tovarTour.categoryid = int.Parse(childnode1.InnerText);
                                                tovarTour.categorytype = childnode1.SelectSingleNode("@type").Value;
                                            }
                                            if (childnode1.Name == "picture") tovarTour.picture = childnode1.InnerText;
                                            if (childnode1.Name == "delivery") tovarTour.delivery = bool.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "local_delivery_cost") tovarTour.local_delivery_cost = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "worldRegion") tovarTour.worldRegion = childnode1.InnerText;
                                            if (childnode1.Name == "country") tovarTour.country = childnode1.InnerText;
                                            if (childnode1.Name == "region") tovarTour.region = childnode1.InnerText;
                                            if (childnode1.Name == "days") tovarTour.days = int.Parse(childnode1.InnerText);                                         
                                            if (childnode1.Name == "country") tovarTour.country = childnode1.InnerText;
                                            if (childnode1.Name == "title") tovarTour.title = childnode1.InnerText;                                           
                                            if (childnode1.Name == "name") tovarTour.name = childnode1.InnerText;
                                            if (childnode1.Name == "hotel_stars") tovarTour.hotel_stars = childnode1.InnerText;
                                            if (childnode1.Name == "room") tovarTour.room = childnode1.InnerText;
                                            if (childnode1.Name == "meal") tovarTour.meal = childnode1.InnerText;
                                            if (childnode1.Name == "included") tovarTour.included = childnode1.InnerText;
                                            if (childnode1.Name == "transport") tovarTour.transport = childnode1.InnerText;
                                            if (childnode1.Name == "description") tovarTour.description = childnode1.InnerText;
                                        }
                                        i++;
                                        offers[i] = tovarTour;
                                        // приведение к форме вывода date only
                                        string date0 = tovarTour.dataTour0.ToShortDateString();
                                        string date1 = tovarTour.dataTour1.ToShortDateString();
                                        Console.WriteLine("\n\nТовар : " + (i + 1));
                                        Console.WriteLine("Идентификационный номер : " + tovarTour.id);
                                        Console.WriteLine("Тип: " + tovarTour.type);
                                        Console.WriteLine("BID : " + tovarTour.bid);
                                        Console.WriteLine("Доступность : " + tovarTour.available);
                                        Console.WriteLine("URL : " + tovarTour.url);
                                        Console.WriteLine("Цена : " + tovarTour.price);
                                        Console.WriteLine("Валюта : " + tovarTour.currencyid);
                                        Console.WriteLine("ID категории : " + tovarTour.categoryid);
                                        Console.WriteLine("Категория : " + tovarTour.categorytype);
                                        Console.WriteLine("Изображение : " + tovarTour.picture);
                                        Console.WriteLine("Доставка : " + tovarTour.delivery);
                                        Console.WriteLine("Стоимость доставки : " + tovarTour.local_delivery_cost);
                                        Console.WriteLine("Материк: " + tovarTour.worldRegion);
                                        Console.WriteLine("Страна : " + tovarTour.country);
                                        Console.WriteLine("Регион : " + tovarTour.region);
                                        Console.WriteLine("Дней : " + tovarTour.days);
                                        Console.WriteLine("Дата начала : " + date0);
                                        Console.WriteLine("Дата конца : " + date1);
                                        Console.WriteLine("Название: " + tovarTour.name);
                                        Console.WriteLine("Количество звезд отеля : " + tovarTour.hotel_stars);
                                        Console.WriteLine("Комната : " + tovarTour.room);
                                        Console.WriteLine("Питание : " +tovarTour.meal);
                                        Console.WriteLine("Включено : " + tovarTour.included);
                                        Console.WriteLine("Транспорт : " + tovarTour.transport);
                                        Console.WriteLine("Описание : " + tovarTour.description);
                                        break;
                                    case "event-ticket":
                                        event_ticket tovarticket = new event_ticket();
                                        tovarticket.id = int.Parse(n.SelectSingleNode("@id").Value);
                                        tovarticket.type = n.SelectSingleNode("@type").Value;
                                        tovarticket.bid = int.Parse(n.SelectSingleNode("@bid").Value);
                                        tovarticket.available = bool.Parse(n.SelectSingleNode("@available").Value);                                      
                                        foreach (XmlNode childnode1 in n)
                                        {
                                            // Console.WriteLine("Ищу в tovarticket");
                                            if (childnode1.Name == "url") tovarticket.url = childnode1.InnerText;
                                            if (childnode1.Name == "price") tovarticket.price = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "currencyId") tovarticket.currencyid = childnode1.InnerText;
                                            if (childnode1.Name == "categoryId")
                                            {
                                                tovarticket.categoryid = int.Parse(childnode1.InnerText);
                                                tovarticket.categorytype = childnode1.SelectSingleNode("@type").Value;
                                            }
                                            if (childnode1.Name == "picture") tovarticket.picture = childnode1.InnerText;
                                            if (childnode1.Name == "delivery") tovarticket.delivery = bool.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "local_delivery_cost") tovarticket.local_delivery_cost = int.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "name") tovarticket.name = childnode1.InnerText;
                                            if (childnode1.Name == "place") tovarticket.place = childnode1.InnerText;
                                            if (childnode1.Name == "hall_part")  tovarticket.hall_part = childnode1.InnerText;
                                            if (childnode1.Name == "hall") tovarticket.hall = childnode1.InnerText;
                                            if (childnode1.Name == "hall") tovarticket.hall_plan = childnode1.SelectSingleNode("@plan").Value;
                                            if (childnode1.Name == "date") tovarticket.date = DateTime.Parse(childnode1.InnerText);
                                            if (childnode1.Name == "is_premiere") 
                                            {
                                                if (childnode1.InnerText == "0") tovarticket.is_premiere = false;
                                                else tovarticket.is_premiere = true;
                                               // Console.WriteLine("ЗНАЧЕНИЕ "+ childnode1.InnerText);
                                            }
                                            if (childnode1.Name == "is_kids")
                                            {
                                                if (childnode1.InnerText == "0") tovarticket.id_kids = false;
                                                else tovarticket.id_kids = true;
                                                //Console.WriteLine("ЗНАЧЕНИЕ " + childnode1.InnerText);
                                            }
                                            if (childnode1.Name == "description") tovarticket.description = childnode1.InnerText;
                                        }
                                        i++;
                                        offers[i] = tovarticket;                                        
                                        Console.WriteLine("\n\nТовар : " + (i + 1));
                                        Console.WriteLine("Идентификационный номер : " + tovarticket.id);
                                        Console.WriteLine("Тип: " + tovarticket.type);
                                        Console.WriteLine("BID : " + tovarticket.bid);
                                        Console.WriteLine("Доступность : " + tovarticket.available);
                                        Console.WriteLine("URL : " + tovarticket.url);
                                        Console.WriteLine("Цена : " + tovarticket.price);
                                        Console.WriteLine("Валюта : " + tovarticket.currencyid);
                                        Console.WriteLine("ID категории : " + tovarticket.categoryid);
                                        Console.WriteLine("Категория : " + tovarticket.categorytype);
                                        Console.WriteLine("Изображение : " + tovarticket.picture);
                                        Console.WriteLine("Доставка : " + tovarticket.delivery);
                                        Console.WriteLine("Стоимость доставки : " + tovarticket.local_delivery_cost);
                                        Console.WriteLine("Название: " + tovarticket.name);
                                        Console.WriteLine("Материк: " + tovarticket.place);
                                        Console.WriteLine("Страна : " + tovarticket.hall);
                                        Console.WriteLine("Регион : " + tovarticket.hall_plan);
                                        Console.WriteLine("Дней : " + tovarticket.hall_part);
                                        Console.WriteLine("Дата начала : " + tovarticket.date);                                                                            
                                        Console.WriteLine("Премьерa : " + tovarticket.is_premiere);
                                        Console.WriteLine("Для детей: " + tovarticket.id_kids);
                                        Console.WriteLine("Описание : " + tovarticket.description);
                                        break;

                                    default:
                                        Console.WriteLine("Default case");
                                        break;
                                }
                                                                                                 
                            }
                        
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Корневой элемент был изменен!");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода");
            Console.ReadKey();                  
      
        }
       
        // создание класса
        public class Offer
        {
            public int id, price, categoryid, bid;
            public string type, url, categorytype, picture, currencyid, description;
            public bool available, delivery;
        }
        // создание класса-наследника
        public class Vendormodel: Offer
        {
            public string  typePrefix,
                          vendor, vendorCode, model, country_of_origin;
            public int local_delivery_cost, cid;
            public bool manufacturer_warranty;         
           
        }

        public class book: Offer
        {
            public string author, name, publisher, series, ISBN, 
                          language, binding;
            public int local_delivery_cost, volume, part, page_extent;
            public bool downloadable;
            public int year;         
        }

        public class audiobook: Offer
        {
            public string author, name,publisher, ISBN, language, perfomed_by, 
                          perfomance_type, storage, format, recording_length;
            public int year;
            public bool downadable;
        }

        public class artisttitle : Offer
        {
            public string title, artist, media, starring, director, country, originaName;                       
            public int year;            
        }

        public class tour : Offer
        {
            public string title,country,worldRegion, region,
                          name, hotel_stars,room, meal,
                          included, transport;
            public DateTime dataTour0, dataTour1;
            public int local_delivery_cost,days;
        }

        public class event_ticket : Offer
        {
            public string name, place, hall_plan, hall, hall_part;
            public DateTime date;
            public int local_delivery_cost; 
            public bool is_premiere, id_kids;
        }      

      
    }
}
