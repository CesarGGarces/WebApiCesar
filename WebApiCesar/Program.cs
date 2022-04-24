using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApiCesar.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

app.MapGet("/ep1/{val}", async (bool val) =>
//app.MapGet("/ep1", async () =>
{
    try
    {
        HttpClient cliente = new HttpClient();
        var respuesta = await cliente.GetAsync("https://api.publicapis.org/entries");
        respuesta.EnsureSuccessStatusCode();
        string ret = await respuesta.Content.ReadAsStringAsync();

        EntJson? ent = JsonSerializer.Deserialize<EntJson>(ret);

        var q = from di in ent.entries
                where di.HTTPS == val
                select di;

        List<Entrada> resHttps = new List<Entrada>();
        foreach (var rHttps in q)
        {
            resHttps.Add(new Entrada()
            {
                API = rHttps.API,
                Description = rHttps.Description,
                Auth = rHttps.Auth,
                HTTPS = rHttps.HTTPS,
                Cors = rHttps.Cors,
                Link = rHttps.Link,
                Category = rHttps.Category
            });




        }

        string d = JsonSerializer.Serialize(resHttps);

        return d;
    }
    catch (Exception e) {
        File.WriteAllText("LogApiCesar.txt",e.Message);
        return e.Message;
    }

});

app.MapGet("/ep2", async() =>
{
    HttpClient cliente = new HttpClient();
    var respuesta = await cliente.GetAsync("https://api.publicapis.org/entries");
    respuesta.EnsureSuccessStatusCode();
    string ret = await respuesta.Content.ReadAsStringAsync();

    EntJson? ent = JsonSerializer.Deserialize<EntJson>(ret);

    var x = 0;
    var cat0 = "";
    var cat = "";
    List<Distinta> distinta = new List<Distinta>();
    foreach (var entrada in ent.entries) {
        cat0 = entrada.Category;
        if (cat != cat0 && cat != "")
        {
            distinta.Add(new Distinta() { Category = cat, Count = x });
            x = 0;
        }
        cat = entrada.Category;
        x++;
    }
  
    string d = JsonSerializer.Serialize(distinta);

    return d;

});

app.Run();
