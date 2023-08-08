#!/bin/bash

./build-all.sh $1

if [ $? -eq 0 ]
then 
    echo "Uploading builds to itch"
    ./upload-itchio-all.sh $1
else
    echo "Some of the builds failed, not uploading."
fi