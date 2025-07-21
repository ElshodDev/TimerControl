var builder = WebApplication.CreateBuilder(args);

// Blazor server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// API manzilini environment-dan olish
string apiBaseUrl = builder.Configuration["API_BASE_URL"] ?? "http://localhost:8080/api/";

// TimerApi HttpClient ro‘yxatdan o‘tkazish
builder.Services.AddHttpClient("TimerApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
