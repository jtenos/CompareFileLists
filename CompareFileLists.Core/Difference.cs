namespace CompareFileLists.Core;

public record struct Difference(ObjectInfo? ObjectInfo1, ObjectInfo? ObjectInfo2, DifferenceType DifferenceType);
