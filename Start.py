import subprocess
import time
import os

print("Starting...")

print("Opening Client Project")
subprocess.Popen(["C:/Program Files/Unity/Hub/Editor/2019.4.2f1/Editor/Unity.exe", "-projectPath", "/Client"])
print("Success")
print("Opening Server Project")
subprocess.Popen(["C:/Program Files/Unity/Hub/Editor/2019.4.2f1/Editor/Unity.exe", "-projectPath", "/Server"])
print("Success")

print("Opening Client Sln")
os.system("start Client/Client.sln")
print("Success")
print("Opening Server Sln")
os.system("start Server/Server.sln")
print("Success")

print("Done")

time.sleep(2)