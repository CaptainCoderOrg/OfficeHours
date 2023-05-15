// See https://aka.ms/new-console-template for more information



PrintSeason(Season.Winter);
PrintSeason(Season.Fall);

int asInteger = (int)Season.Fall;
Console.WriteLine($"{Season.Fall} is actuall {asInteger}");

Season asSeason = (Season)2;
Console.WriteLine($"2 is {asSeason}");

Season asSeason2 = (Season)42;
Console.WriteLine($"42 is {asSeason2}");


void PrintSeason(Season season)
{
    Console.WriteLine($"It is {season}");
}

void DisplaySeasonInfo(Season season)
{
    if (season == Season.Summer) { Console.WriteLine("Let's go to the beach"); }
    else if (season == Season.Winter) { Console.WriteLine("Oh the weather outside is frightful"); }
    else if (season == Season.Spring) { Console.WriteLine("Watch out for allergies"); }
    else if (season == Season.Fall) { Console.WriteLine("Not too hot, not too cold"); }
    else { Console.WriteLine($"I've never heard of {season}"); }
}

[Flags]
enum Season
{
    Winter = 1,
    Spring = 2,
    Summer = 4,
    Fall = 8,
    WinterOrSpring = Winter | Spring
}
