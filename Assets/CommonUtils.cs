public class CommonUtils {
    public static int getNextInt_wrappedAround(int start, int end, int current) {
        return ++current <= end ? current : start;
    }
}
