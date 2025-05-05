#!/usr/bin/env bash

./export.sh

rsync -av --delete Exporter/out/Storage ../Tile3rd/Assets/Scripts/GameLogic