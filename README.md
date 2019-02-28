# Profile Picture Service

This repository contains an ASP.NET Core web API to retrieve users' profile
pictures. Pictures are retrieved from Azure blob storage and verified using
checksums from table storage. Valid profile pictures are returned to the
caller as base 64 strings in JSON objects.

It isn't as fast as we wish it was, though... 
ASP.NET Core is supposed to be fast. What could be wrong?
