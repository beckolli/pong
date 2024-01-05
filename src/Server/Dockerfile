FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
EXPOSE 11000

WORKDIR /app
COPY ../ ./

RUN dotnet restore
RUN dotnet publish \
	--self-contained true \
	-c Release \
	-o out \
	-p:PublishTrimmed=true \
	-p:PublishSingleFile=true

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV DOTNET_RUNNING_IN_CONTAINER=true
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["./"]