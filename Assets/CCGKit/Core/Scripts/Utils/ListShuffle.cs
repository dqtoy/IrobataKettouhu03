using System;
using System.Collections.Generic;

/// <summary>
/// Utility class that defines an extension method for generic lists which provides random shuffling
/// based on the Fisher-Yates shuffle algorithm (https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle).
///
/// This is primarily used by the server when authoritatively shuffling the player decks.
/// Fisher-Yates�V���b�t���A���S���Y���ihttps://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle�j�Ɋ�Â��ă����_���V���b�t�����O��񋟂���ėp���X�g�̊g�����\�b�h���`���郆�[�e�B���e�B�N���X�B
/// ����͎�Ƀv���C���[�f�b�L�𐳎��ɃV���b�t������Ƃ��ɃT�[�o�[�ɂ���Ďg�p����܂��B
/// </summary>
public static class ListShuffle
{
    private static Random rng = new Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            var k = (rng.Next(0, n) % n);
            n--;
            var x = list[k];
            list[k] = list[n];
            list[n] = x;
        }
    }
}