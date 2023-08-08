#!/bin/bash

./build-html5.sh $1

if [ $? -ne 0 ]
then
    exit 1
fi

./build-windows.sh $1

if [ $? -ne 0 ]
then
     exit 1
fi

#./build-macos.sh $1

./build-linux.sh $1

if [ $? -ne 0 ]
then
     exit 1
fi