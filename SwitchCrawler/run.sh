#!/usr/bin/env bash
Xvfb :1 &
cd ./build/
DISPLAY=:1 mono SwitchCrawler.exe