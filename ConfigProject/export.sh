#!/usr/bin/env bash

work_path=$(cd "$(dirname "$0")";pwd)
work_path=$work_path/Exporter
if [ ! -d $work_path ]; then
  mkdir $work_path
fi
cd $work_path
dotnet run -p Exporter.csproj