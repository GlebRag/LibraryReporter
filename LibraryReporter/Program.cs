using Library.Data.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Data.Repositories;
using LibraryReporter.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(AuthService.AUTH_TYPE_KEY)
    .AddCookie(AuthService.AUTH_TYPE_KEY, config =>
    {
        config.LoginPath = "/Auth/Login";
        config.AccessDeniedPath = "/Book/Forbidden";
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<WebDbContext>(x => x.UseSqlServer(WebDbContext.CONNECTION_STRING));

builder.Services.AddScoped<IUserRepositryReal, UserRepository>();
builder.Services.AddScoped<IBookRepositoryReal, BookRepository>();
builder.Services.AddScoped<IReaderRepositoryReal, ReaderRepository>();
builder.Services.AddScoped<IAuthorRepositoryReal, AuthorRepository>();
builder.Services.AddScoped<IPublisherRepositoryReal, PublisherRepository>();
builder.Services.AddScoped<IIssuedBookRepositoryReal, IssuedBookRepository>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EnumHelper>();
builder.Services.AddScoped<UserService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

var seed = new Seed();
seed.Fill(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Book/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=Index}/{id?}");

app.Run();
