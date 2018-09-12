using System.Collections;
using System.Collections.Generic;

public static class Utility  {

    public static T[] ShuffleArray<T>(T[] array, int seed) {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length-1; i ++) {
            //임의의 index를 가져온다 
            int randomIndex = prng.Next(i , array.Length);
            //랜덤값을 temp에 저장
            T tempItem = array[randomIndex];
            //반복문의 현재 위치에 해당하는 값을 randomIndex 위치로 이동
            array[randomIndex] = array[i];
            //현재 반복문의 위치에 randomIndex 값을 설정
            array[i] = tempItem;
        }

        return array;
    }
}
