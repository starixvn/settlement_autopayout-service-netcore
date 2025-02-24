#pragma warning disable S3261 // Namespaces should not be empty
using AutoPayout.Core.Models;

namespace AutoEarlyRefund.Contracts;
#pragma warning restore S3261 // Namespaces should not be empty


public record MatchCalculated(
    int MatchId,
    int ParentId,
    short ShortType,
    short ChildMatchType,
    int? HalfTimeHomeScore,
    int? HalfTimeAwayScore,
    int? FinalHomeScore,
    int? FinalAwayScore,
     string Action = "Cal"
    );

public record MainMatchCalculated(
    int MatchId,
    int ParentId,
    short ShortType,
    short ChildMatchType,
    int? HalfTimeHomeScore,
    int? HalfTimeAwayScore,
    int? FinalHomeScore,
    int? FinalAwayScore,
    string Action = "Cal"
);


public record ExtraMatchCalculated(
    int MatchId,
    int ParentId,
    short ShortType,
    short ChildMatchType,
    int? HalfTimeHomeScore,
    int? HalfTimeAwayScore,
    int? FinalHomeScore,
    int? FinalAwayScore,
    string Action = "Cal"
);



public record ResultsCalculated(
    int MatchId,
    short ShortType,
    short ChildMatchType,
    List<ResourceItem> Resources,
    string Action = "Cal");