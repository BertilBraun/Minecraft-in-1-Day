import os
import stat
import time

_cache = {}
BUFSIZE = 8*1024

def cmp(f1, f2, shallow=True):

    s1 = _sig(os.stat(f1))
    s2 = _sig(os.stat(f2))
    if s1[0] != stat.S_IFREG or s2[0] != stat.S_IFREG:
        return False
    if shallow and s1 == s2:
        return True
    if s1[1] != s2[1]:
        return False

    outcome = _cache.get((f1, f2, s1, s2))
    if outcome is None:
        outcome = _do_cmp(f1, f2)
        if len(_cache) > 100:      # limit the maximum size of the cache
            _cache.clear()
        _cache[f1, f2, s1, s2] = outcome
    return outcome

def _sig(st):
    return (stat.S_IFMT(st.st_mode),
            st.st_size,
            st.st_mtime)

def _do_cmp(f1, f2):
    bufsize = BUFSIZE
    with open(f1, 'rb') as fp1, open(f2, 'rb') as fp2:
        while True:
            b1 = fp1.read(bufsize)
            b2 = fp2.read(bufsize)
            if b1 != b2:
                return False
            if not b1:
                return True






file1 = "tempChunkoutputClient.dat"
file2 = "tempChunkoutputServer.dat"

print("Is Same:", cmp(file1, file2));

with open(file1) as f:
    for i in range(5):
        c = f.read(1)
        print('{:08b}'.format(ord(c)))
        
print("File2")
with open(file2) as f:
    for i in range(5):
        c = f.read(1)
        print('{:08b}'.format(ord(c)))
        
time.sleep(2)