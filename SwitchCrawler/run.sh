#!/usr/bin/env bash
Xvfb :1 &
cd ./build/
mono SwitchCrawler.exe