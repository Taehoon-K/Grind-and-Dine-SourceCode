using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //직렬화해서 인스펙터에 보일수있게
public class GameTimestamp
{
    public int year;
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }
    public enum DayOfTheWeek
    {
        
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }
    public Season season;
    public int day;
    public int hour;
    public int minute;

    public GameTimestamp(int year, Season season, int day, int hour, int minute)
    {
        this.year = year;
        this.season = season;
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }

    public void UpdateClock()
    {
        minute++;

        if(minute >= 60)
        {
            minute = 0;
            hour++;
        }

        if(hour >= 24)
        {
            hour = 0;
            day++;
        }

        if(day > 30)
        {
            day = 1;
            if(season == Season.Winter)
            {
                season = Season.Spring;
                year++;
            }
            else
            {
                season++;
            }
            

        }
    }
    public DayOfTheWeek DayOfWeek
    {
        get { return GetDayOfTheWeek(); }
    }
    public DayOfTheWeek GetDayOfTheWeek() //무슨 요일인지 구하는 함수
    {
        //총 일수 구하기
        int dayPassed = YearsToDays(year) + SeasonsToDays(season) + day;

        int dayIndex = dayPassed % 7;
        return (DayOfTheWeek)dayIndex;
    }
    public int GetPassedDay()
    {
        return YearsToDays(year) + SeasonsToDays(season) + day; //총 일수
    }

    //시간 분으로 변환
    public static int HoursToMinutes(int hour)
    {
        return hour * 60;
    }
    public static int DayToHours(int days)
    {
        return days * 24;
    }
    public static int SeasonsToDays(Season season)
    {
        int seasonIndex = (int)season;
        return seasonIndex * 30;
    }
    public static int YearsToDays(int years)
    {
        return (years-1) * 4 * 30;
    }
    // 특정 시간(timestamp)이 다음 날인지 확인
    public bool IsNextDay(GameTimestamp lastTime)
    {
        return year > lastTime.year ||
               (year == lastTime.year && season > lastTime.season) ||
               (year == lastTime.year && season == lastTime.season && day > lastTime.day);
    }
}
