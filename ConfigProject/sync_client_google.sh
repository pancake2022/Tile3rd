#!/usr/bin/env bash

rsync -av GoogleSheetsHelper/res/tile3rd/* ../Tile3rd/Assets/ExtraResources/Config
rsync -av --delete GoogleSheetsHelper/out/cs/Config ../Tile3rd/Assets/Scripts/GameLogic