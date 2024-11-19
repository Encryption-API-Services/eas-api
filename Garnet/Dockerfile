FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy files
RUN curl -L https://github.com/microsoft/garnet/archive/refs/tags/v1.0.39.tar.gz -o garnet.tar.gz \
    && tar -xzf garnet.tar.gz --strip-components=1 \
    && rm garnet.tar.gz

# Copy and publish app and libraries
WORKDIR /source/main/GarnetServer
RUN dotnet publish -c Release -o /app --self-contained false -f net8.0

# Final stage/image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .

# Run GarnetServer with an index size of 128MB
ENTRYPOINT ["/app/GarnetServer", "-i", "128m"]